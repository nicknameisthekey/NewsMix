using System.Collections.Concurrent;
using NewsMix.Abstractions;
using NewsMix.Services;
using NewsMix.UI.Telegram.Models;
using static CallbackActionType;

namespace NewsMix.UI.Telegram;
public class TelegramUI : UserInterface
{
    private readonly UserService _userService;
    private readonly ITelegramApi _telegramApi;
    private readonly IFeedsInformationService _feedInformationService;
    public string UIType => "telegram";

    private ConcurrentDictionary<long, CallbackData[]> CallbackActions = new();
    private ConcurrentDictionary<long, long> SentMessagesByUser = new();

#if DEBUG
    private readonly List<Update> updatesLog = new();
#endif

    public TelegramUI(UserService userService,
    ITelegramApi telegramApi,
    IFeedsInformationService feedInformationService)
    {
        _userService = userService;
        _telegramApi = telegramApi;
        _feedInformationService = feedInformationService;
    }

    public async Task Start()
    {
        while (true)
        {
            var updates = await _telegramApi.GetUpdates();

#if DEBUG
            updatesLog.AddRange(updates);
#endif    

            foreach (var update in updates)
            {
                if (update.CallBack?.CallbackData != null)
                {
                    try
                    {
                        await ProcessCallback(update);
                    }
                    catch { }
                }
                else if (update.Message != null && update.Message.Text != null)
                {
                    if (update.Message.Date < DateTime.Now.AddMinutes(-3))
                        continue;

                    await ProcessTextMessage(update.Message);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(0.5));
        }
    }

    public async Task ProcessCallback(Update update)
    {
        var callbackData = CallbackData.FromCallback(update.CallBack.CallbackData);

        var userId = update.CallBack.Sender.Id;
        if (CallbackActions.TryGetValue
                (userId, out var userCallbacks) == false)
        {
            //todo fail, we ae waiting no callbacks
        }

        CallbackActions.TryRemove(userId, out _);

        var selectedCallback = userCallbacks!.FirstOrDefault(c => c.ID == callbackData.ID);
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
        await _telegramApi.SendMessage(new SendMessageRequest
        {
            Conversation = user,
            Text = message
        });
    }

    public async Task ProcessTextMessage(Message message)
    {
        await SendFeeds(message.Sender.Id);
    }

    private async Task SendFeeds(long userId)
    {
        var callbacks = _feedInformationService.Feeds
                    .Select(f => new CallbackData
                    {
                        ID = Guid.NewGuid().ToString(),
                        CallbackActionType = SendPublicationTypes,
                        Feed = f,
                        Text = f
                    }).ToArray();
        CallbackActions.TryAdd(userId, callbacks);

        await SendNewOrEdit(userId, callbacks, "Выбери источник данных");
    }

    private async Task SendFeedPublicationTypes(long userId, string feed)
    {
        var allUserSubs = await _userService.GetUserSubscriptions(userId.ToString());
        var userSubs = allUserSubs.ContainsKey(feed) ? allUserSubs[feed] : new();
        var allFeedPubs = _feedInformationService.PublicationTypesByFeed[feed];

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

        CallbackActions.TryAdd(userId, allCallbacks);

        await SendNewOrEdit(userId, allCallbacks, "Выбери тип публикаций данных");
    }

    private async Task SubscribeUser(long userId, Subscription sub)
    {
        await _userService.AddSubscription(userId.ToString(), UIType, sub);
        await SendFeeds(userId);
    }

    private async Task UnsubscribeUser(long userId, Subscription sub)
    {
        await _userService.RemoveSubscription(userId.ToString(), sub);
        await SendFeeds(userId);
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
                CallBackData = buttons[i].ToCallback(),
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