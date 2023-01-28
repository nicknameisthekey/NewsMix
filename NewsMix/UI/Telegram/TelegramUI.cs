using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Models;
using NewsMix.Services;
using NewsMix.UI.Telegram.Models;
using static CallbackActionType;

namespace NewsMix.UI.Telegram;
public class TelegramUI : BackgroundService, UserInterface
{
    internal readonly UserService _userService;
    internal readonly ITelegramApi _telegramApi;
    internal readonly SourcesInformation _sourcesInformation;
    private readonly ILogger<TelegramUI>? _logger;
    public string UIType => "telegram";
    private ConcurrentDictionary<long, CallbackData[]> CallbackActions = new();
    private ConcurrentDictionary<long, long> SentMessagesByUser = new();
    public const string GreetinMessage = "Привет! Я умею присылать новости из разных источников. Посмотреть варианты можно по кнопке \"Меню\".";

    public TelegramUI(UserService userService,
    ITelegramApi telegramApi,
    SourcesInformation sourcesInformation,
    ILogger<TelegramUI>? logger = null)
    {
        _userService = userService;
        _telegramApi = telegramApi;
        _sourcesInformation = sourcesInformation;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var update in _telegramApi.GetUpdates(ct))
        {
            try
            {
                if (update.IsCallback)
                {
                    await ProcessCallback(update);
                }
                else if (update.HasTextMessage)
                {
                    if (update.OlderThan(minutes: 3))
                        continue;

                    await ProcessTextMessage(update.Message!);
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "error on telegram update");
            }
        }
    }

    public async Task ProcessCallback(Update update)
    {
        var userId = update.CallBack!.Sender.Id;
        if (CallbackActions.TryGetValue
                (userId, out var userCallbacks) == false)
        {
            //todo fail, we are waiting no callbacks
        }

        CallbackActions.TryRemove(userId, out _);

        var selectedCallback = userCallbacks!.FirstOrDefault(c => c.ID == update.CallBack.CallbackData);
        ArgumentNullException.ThrowIfNull(selectedCallback);

        await (selectedCallback.CallbackActionType switch
        {
            CallbackActionType.SendTopics => SendTopics(userId, selectedCallback.Source),
            Subscribe => SubscribeUser(userId, new(selectedCallback.Source, selectedCallback.Topic)),
            Unsubscribe => UnsubscribeUser(userId, new(selectedCallback.Source, selectedCallback.Topic)),
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

    private async Task ProcessTextMessage(Message message)
    {
        if (message.Text == "/start")
        {
            await SendGreetings(message.Sender!.Id);
        }

        string command = message.Text!.Replace("/", "");
        if (_sourcesInformation.Sources.Any(f => f == command))
        {
            await SendTopics(message.Sender!.Id, command);
        }
    }

    private async Task SendGreetings(long userId)
    {
        await _telegramApi.SendMessage(new SendMessageRequest
        {
            Text = GreetinMessage,
            Conversation = userId.ToString()
        });
    }

    private async Task SendTopics(long userId, string source)
    {
        var userSubs = await _userService.Subscriptions(userId.ToString(), source);
        
        var callbacks = _sourcesInformation.TopicsBySources[source]
            .Select(topics => new CallbackData
            {
                ID = Guid.NewGuid().ToString(),
                CallbackActionType = userSubs.Any(s => s.Topic == topics) ? Unsubscribe : Subscribe,
                Source = source,
                Topic = topics,
                Text = topics
            }).ToArray();

        CallbackActions.TryRemove(userId, out var _);
        CallbackActions.TryAdd(userId, callbacks);

        await SendNewOrEdit(userId, callbacks, "Что интересует?");
    }

    private async Task SubscribeUser(long userId, Subscription sub)
    {
        _logger?.LogWarning("User {userId} subscribed to {subscription}", userId, sub);
        await _userService.AddSubscription(userId.ToString(), UIType, sub);
        await ReplaceKeyboardWithSuccessMessage(userId);
    }

    private async Task UnsubscribeUser(long userId, Subscription sub)
    {
        _logger?.LogWarning("User {userId} unsubscribed from {subscription}", userId, sub);
        await _userService.RemoveSubscription(userId.ToString(), sub);
        await ReplaceKeyboardWithSuccessMessage(userId);
    }

    private async Task ReplaceKeyboardWithSuccessMessage(long userId)
    {
        if (SentMessagesByUser.TryRemove(userId, out var messageId))
        {
            await _telegramApi.EditMessage(new EditMessageText
            {
                MessageId = messageId,
                ChatId = userId,
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

    private async Task SendNewOrEdit(long userId, CallbackData[] callbacks, string text)
    {
        if (SentMessagesByUser.TryGetValue(userId, out var messageId))
        {
            var response = await _telegramApi.EditMessage(new EditMessageText
            {
                MessageId = messageId,
                ChatId = userId,
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
            SentMessagesByUser.TryAdd(userId, response.Result.MessageId);
        }
    }
}