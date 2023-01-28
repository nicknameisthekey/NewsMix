using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsMix.Abstractions;
using NewsMix.Services;
using NewsMix.UI.Telegram.Models;
using static CallbackActionType;

namespace NewsMix.UI.Telegram;
public class TelegramUI : BackgroundService, UserInterface
{
    internal readonly UserService _userService;
    internal readonly ITelegramApi _telegramApi;
    internal readonly FeedsInformation _feedInformation;
    private readonly ILogger<TelegramUI>? _logger;
    public string UIType => "telegram";
    private ConcurrentDictionary<long, CallbackData[]> CallbackActions = new();
    private ConcurrentDictionary<long, long> SentMessagesByUser = new();
    public const string GreetinMessage = "Привет! Я умею присылать новости из разнызх источников. Посмотреть варианты можно по кнопке \"Меню\"."; //todo

    public TelegramUI(UserService userService,
    ITelegramApi telegramApi,
    FeedsInformation feedInformation,
    ILogger<TelegramUI>? logger = null)
    {
        _userService = userService;
        _telegramApi = telegramApi;
        _feedInformation = feedInformation;
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
        if (selectedCallback == null)
        {
            //fail fast
        }

        await (selectedCallback!.CallbackActionType switch
        {
            SendPublicationTypes => SendFeedPublicationTypes(userId, selectedCallback.Feed),
            Subscribe => SubscribeUser(userId, new(selectedCallback.Feed, selectedCallback.PublicationType!)),
            Unsubscribe => UnsubscribeUser(userId, new(selectedCallback.Feed, selectedCallback.PublicationType!)),
            _ => throw new Exception()
        });
    }

    public async Task NotifyUser(string user, string message)
    {
        _logger?.LogWarning("Notified user {userId}, message {message}", user, message);
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
        if (_feedInformation.Feeds.Any(f => f == command))
        {
            await SendFeedPublicationTypes(message.Sender!.Id, command);
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

    private async Task SendFeedPublicationTypes(long userId, string feed)
    {
        var allUserSubs = await _userService.GetUserSubscriptions(userId.ToString());
        var userSubs = allUserSubs.ContainsKey(feed) ? allUserSubs[feed] : new();
        var allFeedPubs = _feedInformation.PublicationTypesByFeed[feed];

        var subscribePubs = allFeedPubs.Except(userSubs);

        var subscibeCallbacks = subscribePubs.Select(p => new CallbackData
        {
            ID = Guid.NewGuid().ToString(),
            CallbackActionType = Subscribe,
            Feed = feed,
            PublicationType = p,
            Text = p
        });

        var unsubsribeCallbacks = userSubs.Select(p => new CallbackData
        {
            ID = Guid.NewGuid().ToString(),
            CallbackActionType = Unsubscribe,
            Feed = feed,
            PublicationType = p,
            Text = p
        });

        var allCallbacks = subscibeCallbacks.Concat(unsubsribeCallbacks).ToArray();

        CallbackActions.TryRemove(userId, out var _);
        CallbackActions.TryAdd(userId, allCallbacks);

        await SendNewOrEdit(userId, allCallbacks, "Что интересует?");
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