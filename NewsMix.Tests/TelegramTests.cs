using FakeItEasy;
using NewsMix.Abstractions;
using NewsMix.Services;
using NewsMix.UI.Telegram;
using NewsMix.UI.Telegram.Models;

public partial class TelegramTests
{
    UserService NewUserSerivceMock => A.Fake<UserService>();
    const long SomeUserID = 1234;

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
    public async Task Feed_options_send_if_start_command_received()
    {
        var tApiMock = new FakeTelegramApi(NewValidTextUpdate("/start"));
        var feedInformation = new FakeFeedsInformation();
        var tUI = new TelegramUI(NewUserSerivceMock, tApiMock, feedInformation);

        await tUI.StartAsync(CancellationToken.None);

        Assert.Single(tApiMock.SentRequests);
        var sentRequest = tApiMock.SentRequests[0];
        Assert.Equal(feedInformation.Feeds.Length, sentRequest.Keyboard.Keyboard.Length);
    }

    private Update NewValidTextUpdate(string text, long userId = SomeUserID)
    {
        return new Update
        {
            Message = new Message
            {
                Text =text,
                Sender = new NewsMix.UI.Telegram.Models.User
                {
                    Id = 123
                },
                Date_Unix = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
            },
        };
    }
}