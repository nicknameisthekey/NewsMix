using NewsMix.Abstractions;

namespace NewsMix.Tests.Mocks;

public class FakeTelegramUI : UserInterface
{
    public Action<string, string, int> OnNotify = delegate { };
    public string UIName => "Telegram";
    public Task NotifyUser(string externalUserId, string text, int notificationTaskId)
    {
        OnNotify(externalUserId, text, notificationTaskId);
        return Task.CompletedTask;
    }
}