using Microsoft.Extensions.Configuration;
using NewsMix.DAL.Entities;
using NewsMix.DAL.Repositories.Abstraction;
using NewsMix.UI.Telegram.Models;

namespace NewsMix.UI.Telegram;
public class TelegramUI : UserInterface
{
    private readonly UserRepository _userRepo;
    private readonly string _botToken;
    private readonly TelegramApi _telegramApi;
    public string UIType => "telegram";
    string[] options = new[] { "hearthstone", "wow", "wow-classic", "overwatch" };

#if DEBUG
    private readonly List<Update> updatesLog = new();

#endif

    public TelegramUI(UserRepository userRepo,
     IConfiguration configuration,
     IHttpClientFactory httpClientFactory)
    {
        _userRepo = userRepo;
        _botToken = configuration["TelegramBotToken"] ?? throw new ArgumentNullException();
        _telegramApi = new TelegramApi(_botToken, httpClientFactory);
    }

    public async Task Start()
    {
        while (true)
        {
            var updates = await _telegramApi.GetUpdates();
            updates = updates.Where(u => u.Message?.Date > DateTime.Now.AddMinutes(-3))
                             .ToList();

#if DEBUG
            updatesLog.AddRange(updates);
#endif    

            foreach (var update in updates)
            {
                if (update.Message != null && update.Message.Text != null)
                {
                    await ProcessTextMessage(update.Message);
                }
            }

            await Task.Delay(1000);
        }
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
            await _telegramApi.SendMessage(new SendMessageRequest
            {
                Conversation = message.Sender.Id.ToString(),
                Text = "Привет, бот находится на стадии разработки :)"
            });
        }
        else if (options.Contains(message.Text.Replace("_", "-").Replace("/", "")))
        {
            var users = await _userRepo.GetUsers();
            var user = users.FirstOrDefault(u => u.UserId == message.Sender.Id.ToString());
            if (user == null)
                user = new DAL.Entities.User
                {
                    UserId = message.Sender.Id.ToString(),
                    UIType = "telegram"
                };

            user.Subscriptions.Add(new UserSubscription
            {
                FeedName = "noob-club",
                PublicationType = message.Text.Replace("_", "-").Replace("/", "")
            });

            await _userRepo.UpsertUser(user);

            await _telegramApi.SendMessage(new SendMessageRequest
            {
                Conversation = message.Sender.Id.ToString(),
                Text = $"Подписался на {message.Text}"
            });
        }
        else
        {
            await _telegramApi.SendMessage(new SendMessageRequest
            {
                Conversation = message.Sender.Id.ToString(),
                Text = $"Я тебя не понял :("
            });
        }
    }
}