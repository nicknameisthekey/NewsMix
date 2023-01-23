using FakeItEasy;
using NewsMix.Services;
using NewsMix.UI.Telegram;
using NewsMix.UI.Telegram.Models;

public partial class TelegramTests
{
    UserService NewUserSerivceMock => A.Fake<UserService>();
    const long SomeUserID = 1234;

    private (TelegramUI TUI, FakeTelegramApi TAPI) NewTelegramUI
        (FakeFeedsInformation? feedsInfo = null, params object[] updates)
    {
        var tApiMock = new FakeTelegramApi(updates);
        return (new TelegramUI(NewUserSerivceMock, tApiMock, feedsInfo ?? new FakeFeedsInformation()), tApiMock);
    }

    [Fact]
    public async Task Old_text_messages_ignored()
    {
        var tApiMock = new FakeTelegramApi(new Update
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
        var tUI = new TelegramUI(NewUserSerivceMock, tApiMock, new FakeFeedsInformation());

        await tUI.StartAsync(CancellationToken.None);

        Assert.Empty(tApiMock.SentRequests);
    }

    [Fact]
    public async Task Start_command_sends_greeting_message()
    {
        var (TUI, TAPI) = NewTelegramUI(new(), ValidTextUpdate("/start"));

        await TUI.StartAsync(CancellationToken.None);

        Assert.Single(TAPI.SentRequests);
        var sentRequest = TAPI.SentRequests[0];
        Assert.Equal(TelegramUI.GreetinMessage, sentRequest.Text);
    }

    [Fact]
    public async Task Publication_types_send_in_response_on_feed_command()
    {
        var feedsInfo = new FakeFeedsInformation();
        string sentFeed = feedsInfo.Feeds[0];
        var (TUI, TAPI) = NewTelegramUI(feedsInfo,
        ValidTextUpdate($"/{sentFeed}"));

        await TUI.StartAsync(CancellationToken.None);

        Assert.Single(TAPI.SentRequests);
        var sentRequest = TAPI.SentRequests[0];
        Assert.Equal(feedsInfo.PublicationTypesByFeed[sentFeed].Length, sentRequest.Keyboard.Keyboard.Length);
    }

    [Fact]
    public async Task User_can_subscribe()
    {
        var feedsInfo = new FakeFeedsInformation();
        string sentFeed = feedsInfo.Feeds[0];
        var (TUI, TAPI) = NewTelegramUI(feedsInfo,
         ValidTextUpdate($"/{sentFeed}"),
          new CallBackUpdate
          {
              ButtonNumber = (0, 0)
          });

        await TUI.StartAsync(CancellationToken.None);
    }
    private Update ValidTextUpdate(string text, long userId = SomeUserID)
    {
        return new Update
        {
            Message = new Message
            {
                Text = text,
                Sender = new NewsMix.UI.Telegram.Models.User
                {
                    Id = 123
                },
                Date_Unix = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
            },
        };
    }
}