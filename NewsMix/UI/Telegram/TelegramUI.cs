using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Services;
using NewsMix.Storage.Entites;
using NewsMix.UI.Telegram.Models;
using static CallbackActionType;

namespace NewsMix.UI.Telegram;
public class TelegramUI : BackgroundService, UserInterface
{
    internal readonly UserService _userService;
    internal readonly ITelegramApi _telegramApi;
    internal readonly IStatsService _statsService;
    internal readonly SourcesInformation _sourcesInformation;
    private readonly ILogger<TelegramUI>? _logger;
    public string UIType => "telegram";
    private ConcurrentDictionary<string, CallbackData[]> CallbackActions = new();
    private ConcurrentDictionary<string, long> SentMessagesByUser = new();
    public const string GreetinMessage = "Привет! Я умею присылать новости из разных источников. Посмотреть варианты можно по кнопке \"Меню\".";

    internal TelegramUI(UserService userService,
    ITelegramApi telegramApi,
    SourcesInformation sourcesInformation,
    IStatsService statsService,
    ILogger<TelegramUI>? logger = null)
    {
        _userService = userService;
        _telegramApi = telegramApi;
        _sourcesInformation = sourcesInformation;
        _logger = logger;
        _statsService = statsService;
    }

    public TelegramUI(IServiceProvider services)
    {
        var scope = services.CreateScope();

        _userService = scope.ServiceProvider.GetRequiredService<UserService>();
        _telegramApi = scope.ServiceProvider.GetRequiredService<ITelegramApi>();
        _sourcesInformation = scope.ServiceProvider.GetRequiredService<SourcesInformation>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<TelegramUI>>();
        _statsService = scope.ServiceProvider.GetRequiredService<IStatsService>();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var update in _telegramApi.GetUpdates(ct))
        {
            try
            {
                var user = new UserModel
                {
                    UserId = update.UserId,
                    Name = update.UserName,
                    UIType = UIType
                };

                if (update.IsCallback)
                {
                    await ProcessCallback(user, update.CallBack!);
                }
                else if (update.HasTextMessage)
                {
                    if (update.OlderThan(minutes: 3))
                        continue;

                    await ProcessTextMessage(user, update.Message!);
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "error on telegram update");
            }
        }
    }

    public async Task ProcessCallback(UserModel user, CallbackQuery callback)
    {
        if (CallbackActions.TryGetValue
                (user.UserId, out var userCallbacks) == false)
        {
            //todo fail, we are waiting no callbacks
        }

        CallbackActions.TryRemove(user.UserId, out _);

        var selectedCallback = userCallbacks!.FirstOrDefault(c => c.ID == callback.CallbackData);
        ArgumentNullException.ThrowIfNull(selectedCallback);

        await (selectedCallback.CallbackActionType switch
        {
            CallbackActionType.SendTopics => SendTopics(user, selectedCallback.Source),
            Subscribe => SubscribeUser(user, new(selectedCallback.Source, selectedCallback.Topic)),
            Unsubscribe => UnsubscribeUser(user, new(selectedCallback.Source, selectedCallback.Topic)),
            _ => throw new Exception()
        });
    }

    public async Task NotifyUser(string user, string message)
    {
        await _telegramApi.SendMessage(new SendMessageRequest
        {
            Conversation = user,
            Text = message
        });
    }

    private async Task ProcessTextMessage(UserModel user, Message message)
    {
        if (message.Text == "/start")
        {
            await SendGreetings(message.Sender!.Id);
        }

        if (message.Text == "/stats")
        {
            await SendStats(message.Sender!.Id);
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
        string text = $"🎉 Нас уже {userCount}\\!" + Environment.NewLine +
                    $"🗞 Новостей отправлено: {notificationsCount}";

        var commitUrl = Environment.GetEnvironmentVariable("GITCOMMITURL");
        if (commitUrl != null)
        {
            text += $"{Environment.NewLine}Собрано из [этого комита]({commitUrl})";
        }

        await _telegramApi.SendMessage(new SendMessageRequest
        {
            Text = text,
            Conversation = userId.ToString(),
            ParseMode = SendMessageRequest.ParseModeMD2
        });
    }

    private async Task SendGreetings(long userId)
    {
        await _telegramApi.SendMessage(new SendMessageRequest
        {
            Text = GreetinMessage,
            Conversation = userId.ToString()
        });
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
            await _telegramApi.EditMessage(new EditMessageText
            {
                MessageId = messageId,
                ChatId = long.Parse(userId),
                Text = "Успешно"
            });
        }
        //todo else case
    }

    private InlineKeyboard CreateKeyboard(CallbackData[] buttons)
    {
        var keyboard = new InlineKeyboard
        {
            Keyboard = new InlineKeyboardButton[buttons.Length, 1]
        };

        for (int i = 0; i < buttons.Length; i++)
        {
            var additionalText = buttons[i].CallbackActionType switch
            {
                Unsubscribe => " (отписаться)",
                _ => ""
            };

            keyboard.Keyboard[i, 0] = new InlineKeyboardButton
            {
                CallBackData = buttons[i].ID,
                Text = buttons[i].Text + additionalText
            };
        }
        return keyboard;
    }

    private async Task SendNewOrEdit(string userId, CallbackData[] callbacks, string text)
    {
        if (SentMessagesByUser.TryGetValue(userId, out var messageId))
        {
            var response = await _telegramApi.EditMessage(new EditMessageText
            {
                MessageId = messageId,
                ChatId = long.Parse(userId),
                Keyboard = CreateKeyboard(callbacks),
                Text = text
            });
        }
        else
        {
            var response = await _telegramApi.SendMessage(new SendMessageRequest
            {
                Conversation = userId.ToString(),
                Keyboard = CreateKeyboard(callbacks),
                Text = text
            });
            SentMessagesByUser.TryRemove(userId, out _);
            SentMessagesByUser.TryAdd(userId, response.Result!.MessageId);
        }
    }
}