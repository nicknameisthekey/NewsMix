using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Services;
using NewsMix.Storage.Entites;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using static NewsMix.UI.Telegram.CallbackActionType;

namespace NewsMix.UI.Telegram;

public class Telegram : BackgroundService, UserInterface
{
    public string UIType => "telegram";

    private ConcurrentDictionary<string, CallbackData[]> CallbackActions = new();
    private ConcurrentDictionary<string, long> SentMessagesByUser = new();
    private readonly ITelegramBotClient _client;
    private readonly SourcesInformation _sourcesInformation;
    private readonly IStatsService _statsService;
    private readonly UserService _userService;
    private readonly ILogger<Telegram>? _logger;

    public Telegram(ITelegramBotClient client,
        IServiceProvider services,
        ILogger<Telegram>? logger = null)
    {
        _client = client;
        var scope = services.CreateScope();
        _sourcesInformation = scope.ServiceProvider.GetRequiredService<SourcesInformation>();
        _statsService = scope.ServiceProvider.GetRequiredService<IStatsService>();
        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
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
        var user = new UserModel
        {
            UserId = (update.CallbackQuery?.From?.Id ??
                      update.Message?.Chat?.Id ??
                      update.Message!.From!.Id)!.ToString(),
            Name = update.CallbackQuery?.From?.Username ??
                   update.Message?.Chat?.Username ??
                   update.Message!.From!.Username ?? "unknown",
            UIType = UIType
        };

        var task = update.Type switch
        {
            UpdateType.CallbackQuery => ProcessCallback(user, update.CallbackQuery!),
            UpdateType.Message when update.Message?.Date > DateTime.UtcNow.AddMinutes(-3) => ProcessTextMessage(user,
                update.Message!),
            _ => Task.CompletedTask
        };

        await task;
    }

    public async Task ProcessCallback(UserModel user, CallbackQuery callback)
    {
        if (CallbackActions.TryGetValue
                (user.UserId, out var userCallbacks) == false)
        {
            //todo fail, we are waiting no callbacks
        }

        CallbackActions.TryRemove(user.UserId, out _);

        var selectedCallback = userCallbacks!.FirstOrDefault(c => c.ID == callback.Data);
        ArgumentNullException.ThrowIfNull(selectedCallback);

        await (selectedCallback.CallbackActionType switch
        {
            CallbackActionType.SendTopics => SendTopics(user, selectedCallback.Source),
            Subscribe => SubscribeUser(user, new(selectedCallback.Source, selectedCallback.Topic)),
            Unsubscribe => UnsubscribeUser(user, new(selectedCallback.Source, selectedCallback.Topic)),
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

        string command = message.Text!.Replace("/", "");
        if (_sourcesInformation.Sources.Any(f => f == command))
        {
            await SendTopics(user, command);
        }
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

        var callbacks = _sourcesInformation.TopicsBySources[source]
            .Select(topics => new CallbackData
            {
                ID = Guid.NewGuid().ToString(),
                CallbackActionType = userSubs.Any(s => s.Topic == topics) ? Unsubscribe : Subscribe,
                Source = source,
                Topic = topics,
                Text = topics
            }).ToArray();

        CallbackActions.TryRemove(user.UserId, out var _);
        CallbackActions.TryAdd(user.UserId, callbacks);

        await SendNewOrEdit(user.UserId, callbacks, "Что интересует?");
    }

    private async Task SubscribeUser(UserModel user, Subscription sub)
    {
        _logger?.LogWarning("User {user} subscribed to {subscription}", user, sub);
        await _userService.AddSubscription(user, sub);
        await ReplaceKeyboardWithSuccessMessage(user.UserId);
    }

    private async Task UnsubscribeUser(UserModel user, Subscription sub)
    {
        _logger?.LogWarning("User {user} unsubscribed from {subscription}", user, sub);
        await _userService.RemoveSubscription(user, sub);
        await ReplaceKeyboardWithSuccessMessage(user.UserId);
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