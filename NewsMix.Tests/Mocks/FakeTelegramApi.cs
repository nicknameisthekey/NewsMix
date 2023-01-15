using NewsMix.UI.Telegram.Models;

public class FakeTelegramApi : ITelegramApi
{
    private readonly Update[] _updates;

    public List<SendMessageRequest> SentRequests = new();
    private int currentResultMessageId = 0;
    public FakeTelegramApi(params Update[] updates)
    {
        _updates = updates;
    }
    public Task<SendMessageResponse> EditMessage(EditMessageText message)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<Update> GetUpdates(CancellationToken ct)
    {
        foreach (var update in _updates)
            yield return update;
    }

    public async Task<SendMessageResponse> SendMessage(SendMessageRequest message)
    {
        SentRequests.Add(message);
        currentResultMessageId++;
        return new SendMessageResponse
        {
            Result = new Result
            {
                MessageId = currentResultMessageId
            },
            Success = true
        };
    }
}