using NewsMix.Models;
using NewsMix.UI.Telegram;
using NewsMix.UI.Telegram.Models;

public partial class TelegramTests : IDisposable
{
    const long SomeUserID = 1234;

    private TelegramUI NewTelegramUI()
        => new TelegramUI(TestHelpers.NewUserService, new FakeTelegramApi(), new FakeSourcesInformation());

    [Fact]
    public async Task Old_text_messages_ignored()
    {
        var tApiMock = new FakeTelegramApi();
        tApiMock.AddUpdate(new Update
        {
            Message = new Message
            {
                Text = "/start",
                Sender = new NewsMix.UI.Telegram.Models.User
                {
                    Id = 123
                },
                Date_Unix = 0
            },
        });
        var tUI = new TelegramUI(TestHelpers.NewUserService, tApiMock, new FakeSourcesInformation());

        await tUI.StartAsync(CancellationToken.None);

        Assert.Empty(tApiMock.SentRequests);
    }

    [Fact]
    public async Task Start_command_sends_greeting_message()
    {
        var TUI = NewTelegramUI();
        TUI._telegramApi.SendTextFromUser("/start");

        await TUI.StartAsync(CancellationToken.None);
        var sentRequests = TUI._telegramApi.GetSentRequests();

        Assert.Single(sentRequests);
        Assert.Equal(TelegramUI.GreetinMessage, sentRequests[0].Text);
    }

    [Fact]
    public async Task Topics_send_in_response_on_source_command()
    {
        var TUI = NewTelegramUI();
        var userChosenSource = TUI._sourcesInformation.Sources[0];

        var userId = TUI._telegramApi.SendTextFromUser($"/{userChosenSource}");
        await TUI.StartAsync(CancellationToken.None);

        var sentRequests = TUI._telegramApi.GetSentRequests();
        Assert.Single(sentRequests);
        Assert.Equal(TUI._sourcesInformation.TopicsBySources[userChosenSource].Length,
                     sentRequests[0]?.Keyboard?.Keyboard.Length);
    }

    [Fact]
    public async Task User_can_subscribe()
    {
        var TUI = NewTelegramUI();
        var userChosenSource = TUI._sourcesInformation.Sources[0];

        var userId = TUI._telegramApi.SendTextFromUser($"/{userChosenSource}");
        TUI._telegramApi.SendButtonPress(0, 0);
        await TUI.StartAsync(CancellationToken.None);

        var userSubs = await TUI._userService.Subscriptions(userId);
        Assert.Single(userSubs);
        var editRequests = TUI._telegramApi.GetEditRequests();
        Assert.Single(editRequests);
        Assert.Null(editRequests[0].Keyboard);   //keyboard removed after sub
    }

    [Fact]
    public async Task User_can_unsubscribe()
    {
        var TUI = NewTelegramUI();
        var userChosenSource = TUI._sourcesInformation.Sources[0];

        var userId = TUI._telegramApi.SendTextFromUser($"/{userChosenSource}");
        var userSub = new Subscription(userChosenSource,
                        TUI._sourcesInformation.TopicsBySources[userChosenSource][0]);
        await TUI._userService.AddSubscription(userId, TUI.UIType, userSub);

        TUI._telegramApi.SendButtonPress(userSub.Topic);
        await TUI.StartAsync(CancellationToken.None);

        var userSubs = await TUI._userService.Subscriptions(userId);
        Assert.Empty(userSubs);
        var editRequests = TUI._telegramApi.GetEditRequests();
        Assert.Single(editRequests);
        Assert.Null(editRequests[0].Keyboard);
    }

    public void Dispose()
    {
        TestHelpers.EmptyTestFilesDirectory();
    }
}

public static partial class TestHelpers
{
    public static string SendTextFromUser(this ITelegramApi api, string command)
    {
        var fakeApi = (FakeTelegramApi)api;
        Update update = new Update
        {
            Message = new Message
            {
                Text = command,
                Sender = new NewsMix.UI.Telegram.Models.User
                {
                    Id = 123
                },
                Date_Unix = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
            },
        };
        fakeApi.AddUpdate(update);
        return update.Message.Sender.Id.ToString();
    }

    public static ITelegramApi SendButtonPress(this ITelegramApi api, int row, int col)
    {
        var fakeApi = (FakeTelegramApi)api;
        fakeApi.AddUpdate(new ButtonPress(row, col));
        return fakeApi;
    }

    public static ITelegramApi SendButtonPress(this ITelegramApi api, string buttonText)
    {
        var fakeApi = (FakeTelegramApi)api;
        fakeApi.AddUpdate(new ButtonPress(buttonText));
        return fakeApi;
    }

    public static List<SendMessageRequest> GetSentRequests(this ITelegramApi api)
    {
        var fakeApi = (FakeTelegramApi)api;
        return fakeApi.SentRequests;
    }

    public static List<EditMessageText> GetEditRequests(this ITelegramApi api)
    {
        var fakeApi = (FakeTelegramApi)api;
        return fakeApi.EditedMessages;
    }
}