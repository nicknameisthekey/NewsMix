using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Services;
using NewsMix.Storage;
using NewsMix.Storage.Entities;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using static NewsMix.UI.Telegram.CallbackActionType;

namespace NewsMix.UI.Telegram;

public class Telegram : BackgroundService, UserInterface
{
    public string UIName => "telegram";

    private readonly ConcurrentDictionary<string, long> SentMessagesByUser = new();
    private readonly ITelegramBotClient _client;
    private readonly IStatsService _statsService;
    private readonly UserService _userService;
    private readonly ILogger<Telegram>? _logger;
    private readonly SqliteContext _context;

    public Telegram(ITelegramBotClient client,
        IServiceProvider services, ILogger<Telegram>? logger = null)
    {
        _client = client;
        var scope = services.CreateScope();
        _statsService = scope.ServiceProvider.GetRequiredService<IStatsService>();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _context = scope.ServiceProvider.GetRequiredService<SqliteContext>();
        _logger = logger;
    }

    public const string GreetinMessage =
        "Привет! Я умею присылать новости из разных источников. Посмотреть варианты можно по кнопке \"Меню\".";


    public async Task NotifyUser(string user, string message)
    {
        await _client.SendTextMessageAsync(chatId: user, text: message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _client.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient arg1, Exception ex, CancellationToken arg3)
    {
        _logger?.LogError(ex, "Error while polling");
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient c, Update update, CancellationToken arg3)
    {
        try
        {
            _logger?.LogInformation("got update {@update}", update);
            var user = new UserModel
            {
                ExternalUserId = (update.CallbackQuery?.From?.Id ??
                                  update.Message?.Chat?.Id ??
                                  update.Message!.From!.Id)!.ToString(),
                Name = update.CallbackQuery?.From?.Username ??
                       update.Message?.Chat?.Username ??
                       update.Message!.From!.Username ?? "unknown",
                UIType = UIName
            };

            if (user.ExternalUserId == null)
                throw new Exception("Could not find userId in update");

            var task = update.Type switch
            {
                UpdateType.CallbackQuery => ProcessCallback(user, update.CallbackQuery!),
                UpdateType.Message when update.Message?.Date > DateTime.UtcNow.AddMinutes(-3) => ProcessTextMessage(
                    user,
                    update.Message!),
                _ => Task.CompletedTask
            };

            await task;
        }
        catch (Exception e)
        {
            _logger?.LogError(e, $"Error on processing update {JsonConvert.SerializeObject(update)}");
        }
    }

    public async Task ProcessCallback(UserModel user, CallbackQuery callback)
    {
        var activeQueries = await _context.ActiveInlineQueries
            .Where(q => q.ExternalUserId == user.ExternalUserId)
            .ToListAsync();

        if (activeQueries.Any() == false)
        {
            _logger?.LogError("callback not found for user with {externalId}", user.ExternalUserId);
            //todo fail?
        }

        _context.ActiveInlineQueries.RemoveRange(activeQueries);
        await _context.SaveChangesAsync();

        var selectedQuery = activeQueries.Single(c => c.QueryID == callback.Data);

        await (selectedQuery.CallbackActionType switch
        {
            CallbackActionType.SendTopics => SendTopics(user, selectedQuery.Source),
            Subscribe => SubscribeUser(user, new(selectedQuery.Source, selectedQuery.TopicInternalName)),
            Unsubscribe => UnsubscribeUser(user, new(selectedQuery.Source, selectedQuery.TopicInternalName)),
            _ => throw new Exception()
        });
    }

    private async Task ProcessTextMessage(UserModel user, Message message)
    {
        if (message.Text == "/start")
        {
            await SendGreetings(message.From!.Id);
        }

        if (message.Text == "/stats")
        {
            await SendStats(message.From!.Id);
        }

        if (message.Text == "/stop")
        {
            await UnsubscribeEverything(user);
        }

        var command = message.Text!.Replace("/", "");
        if (await _context.NewsTopics.AnyAsync(t => t.NewsSource == command))
        {
            await SendTopics(user, command);
        }
    }

    private async Task UnsubscribeEverything(UserModel user)
    {
        var subs = await _userService.Subscriptions(user);
        foreach (var sub in subs.ToArray())
            await _userService.RemoveSubscription(user, sub);

        await _client.SendTextMessageAsync(user.ExternalUserId, "Успешно");
    }

    private async Task SendStats(long userId)
    {
        var userCount = await _statsService.UsersCount();
        var notificationsCount = await _statsService.NotificationsCount();
        var text = $"🎉 Нас уже {userCount}\\!" + Environment.NewLine +
                   $"🗞 Новостей отправлено: {notificationsCount}";

        var commitUrl = Environment.GetEnvironmentVariable("GITCOMMITURL");
        if (commitUrl != null)
        {
            text += $"{Environment.NewLine}Собрано из [этого комита]({commitUrl})";
        }

        await _client.SendTextMessageAsync(userId, text, parseMode: ParseMode.MarkdownV2);
    }

    private async Task SendGreetings(long userId)
    {
        await _client.SendTextMessageAsync(userId.ToString(), GreetinMessage);
    }

    private async Task SendTopics(UserModel user, string source)
    {
        var userSubs = await _userService.Subscriptions(user, source);
        //todo check order can be changed while running
        var topicsBySource = (await _context.NewsTopics.ToListAsync())
            .GroupBy(t => t.NewsSource)
            .ToDictionary(t => t.Key);

        var callbacks = topicsBySource[source]
            .OrderBy(t => t.OrderInList)
            .Select(topic => new CallbackData
            {
                ID = Guid.NewGuid().ToString(),
                CallbackActionType = userSubs.Any(s => s.TopicInternalName == topic.InternalName)
                    ? Unsubscribe
                    : Subscribe,
                Source = source,
                TopicInternalName = topic.InternalName,
                Text = topic.VisibleNameRU
            }).ToArray();
        
        await _context.ActiveInlineQueries
            .Where(q => q.ExternalUserId == user.ExternalUserId)
            .ExecuteDeleteAsync();

        _context.ActiveInlineQueries.AddRange(callbacks.Select(c => new ActiveInlineQuery()
        {
            ExternalUserId = user.ExternalUserId,
            CallbackActionType = c.CallbackActionType,
            QueryID = c.ID,
            Source = c.Source,
            TopicInternalName = c.TopicInternalName
        }));
        await _context.SaveChangesAsync();

        await SendNewOrEdit(user.ExternalUserId, callbacks, "Что интересует?");
    }

    private async Task SubscribeUser(UserModel user, Subscription sub)
    {
        _logger?.LogWarning("User {user} subscribed to {subscription}", user, sub);
        await _userService.AddSubscription(user, sub);
        await ReplaceKeyboardWithSuccessMessage(user.ExternalUserId);
    }

    private async Task UnsubscribeUser(UserModel user, Subscription sub)
    {
        _logger?.LogWarning("User {user} unsubscribed from {subscription}", user, sub);
        await _userService.RemoveSubscription(user, sub);
        await ReplaceKeyboardWithSuccessMessage(user.ExternalUserId);
    }

    private async Task ReplaceKeyboardWithSuccessMessage(string userId)
    {
        if (SentMessagesByUser.TryRemove(userId, out var messageId))
        {
            await _client.EditMessageTextAsync(chatId: userId, messageId: (int)messageId, text: "Успешно");
        }
        //todo else case
    }

    private InlineKeyboardMarkup CreateKeyboard(CallbackData[] callBacks)
    {
        var rows = callBacks.Select(c =>
        {
            var finalText = c.CallbackActionType switch
            {
                Unsubscribe => c.Text + " (отписаться)",
                _ => c.Text
            };

            return new List<InlineKeyboardButton>() { InlineKeyboardButton.WithCallbackData(finalText, c.ID) };
        });

        return new InlineKeyboardMarkup(rows);
    }

    private async Task SendNewOrEdit(string userId, CallbackData[] callbacks, string text)
    {
        if (SentMessagesByUser.TryGetValue(userId, out var messageId))
        {
            var response = await _client.EditMessageTextAsync(chatId: userId,
                messageId: (int)messageId,
                text: text,
                replyMarkup: CreateKeyboard(callbacks));
        }
        else
        {
            var response =
                await _client.SendTextMessageAsync(chatId: userId, text: text, replyMarkup: CreateKeyboard(callbacks));

            SentMessagesByUser.TryRemove(userId, out _);
            SentMessagesByUser.TryAdd(userId, response!.MessageId);
        }
    }
}