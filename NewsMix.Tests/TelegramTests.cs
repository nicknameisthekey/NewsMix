using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using User = Telegram.Bot.Types.User;

namespace NewsMix.Tests;

public class TelegramTests
{
    private readonly IConfiguration config = TestHelpers.PrepareTestEnv();
    
    [Fact]
    public async Task Old_text_messages_ignored()
    {
        var client = A.Fake<ITelegramBotClient>();
        A.CallTo(() => client.MakeRequestAsync((SendMessageRequest)null!, default))
            .WithAnyArguments()
            .Throws(new Exception());
        
        var services = new ServiceCollection()
            .AddNewsMix(config, false)
            .WithSingletonMock(client)
            .AddSingleton<UI.Telegram.Telegram>()
            .CreateScope();
        
        var telegram = services.GetRequiredService<UI.Telegram.Telegram>();

        await telegram.HandleUpdateAsync(null!, new Update
        {
            Message = new Message
            {
                Text = "/start",
                From = new User
                {
                    Id = 1234
                },
                Date = DateTime.UtcNow.AddMinutes(-4),
            }
        }, default);
    }
    
    //todo: rewrite for fake ITelegramClient
//
//     [Fact]
//     public async Task Start_command_sends_greeting_message()
//     {
//         var TUI = NewTelegramUI();
//         TUI._telegramApi.SendTextFromUser("/start");
//
//         await TUI.StartAsync(CancellationToken.None);
//         var sentRequests = TUI._telegramApi.GetSentRequests();
//
//         Assert.Single(sentRequests);
//         Assert.Equal(TelegramUI.GreetinMessage, sentRequests[0].Text);
//     }
//
//     [Fact]
//     public async Task Topics_send_in_response_on_source_command()
//     {
//         var TUI = NewTelegramUI();
//         var userChosenSource = TUI._sourcesInformation.Sources[0];
//
//         var userId = TUI._telegramApi.SendTextFromUser($"/{userChosenSource}");
//         await TUI.StartAsync(CancellationToken.None);
//
//         var sentRequests = TUI._telegramApi.GetSentRequests();
//         Assert.Single(sentRequests);
//         Assert.Equal(TUI._sourcesInformation.TopicsBySources[userChosenSource].Length,
//             sentRequests[0]?.Keyboard?.Keyboard.Length);
//     }
//
//     [Fact]
//     public async Task User_can_subscribe()
//     {
//         var TUI = NewTelegramUI();
//         var userChosenSource = TUI._sourcesInformation.Sources[0];
//
//         var user = TUI._telegramApi.SendTextFromUser($"/{userChosenSource}");
//         TUI._telegramApi.SendButtonPress(0, 0);
//         await TUI.StartAsync(CancellationToken.None);
//
//         var userSubs = await TUI._userService.Subscriptions(user);
//         Assert.Single(userSubs);
//         var editRequests = TUI._telegramApi.GetEditRequests();
//         Assert.Single(editRequests);
//         Assert.Null(editRequests[0].Keyboard); //keyboard removed after sub
//     }
//
//     [Fact]
//     public async Task User_can_unsubscribe()
//     {
//         var TUI = NewTelegramUI();
//         var userChosenSource = TUI._sourcesInformation.Sources[0];
//
//         var user = TUI._telegramApi.SendTextFromUser($"/{userChosenSource}");
//         var userSub = new Subscription
//         {
//             Source = userChosenSource,
//             Topic = TUI._sourcesInformation.TopicsBySources[userChosenSource][0]
//         };
//         await TUI._userService.AddSubscription(user, userSub);
//
//         TUI._telegramApi.SendButtonPress(userSub.Topic);
//         await TUI.StartAsync(CancellationToken.None);
//
//         Assert.Empty(await TUI._userService.Subscriptions(user));
//         var editRequests = TUI._telegramApi.GetEditRequests();
//         Assert.Single(editRequests);
//         Assert.Null(editRequests[0].Keyboard);
//     }
// }
//
// public static partial class TestHelpers
// {
//     public static UserModel SendTextFromUser(this ITelegramApi api, string command)
//     {
//         var fakeApi = (FakeTelegramApi)api;
//         Update update = new Update
//         {
//             Message = new Message
//             {
//                 Text = command,
//                 Sender = new NewsMix.UI.Telegram.Models.User
//                 {
//                     Id = 123,
//                     UserName = "1234"
//                 },
//                 Date_Unix = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
//             },
//         };
//         fakeApi.AddUpdate(update);
//         return new UserModel
//         {
//             UserId = update.Message.Sender.Id.ToString(),
//             Name = update.Message.Sender.UserName,
//             UIType = "telegram"
//         };
//     }
//
//     public static ITelegramApi SendButtonPress(this ITelegramApi api, int row, int col)
//     {
//         var fakeApi = (FakeTelegramApi)api;
//         fakeApi.AddUpdate(new ButtonPress(row, col));
//         return fakeApi;
//     }
//
//     public static ITelegramApi SendButtonPress(this ITelegramApi api, string buttonText)
//     {
//         var fakeApi = (FakeTelegramApi)api;
//         fakeApi.AddUpdate(new ButtonPress(buttonText));
//         return fakeApi;
//     }
//
//     public static List<SendMessageRequest> GetSentRequests(this ITelegramApi api)
//     {
//         var fakeApi = (FakeTelegramApi)api;
//         return fakeApi.SentRequests;
//     }
//
//     public static List<EditMessageText> GetEditRequests(this ITelegramApi api)
//     {
//         var fakeApi = (FakeTelegramApi)api;
//         return fakeApi.EditedMessages;
//     }
}