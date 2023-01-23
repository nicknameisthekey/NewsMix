using NewsMix.UI.Telegram.Models;

public class CallBackUpdate
{
    public (int Row, int Col) ButtonNumber { get; init; }
}

public class FakeTelegramApi : ITelegramApi
{
    private readonly object[] _updates;

    public List<SendMessageRequest> SentRequests = new();
    private int currentResultMessageId = 0;

    public FakeTelegramApi(params object[] updates)
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
        {
            if (update is Update u)
            {
                yield return u;
            }
            else if (update is CallBackUpdate cu)
            {
                var button = SentRequests.Last().Keyboard
                    .Keyboard[cu.ButtonNumber.Col, cu.ButtonNumber.Row] as InlineKeyboardButton;
                yield return new Update
                {
                    CallBack = new CallbackQuery
                    {
                        CallbackData = button!.CallBackData,
                        Sender = new User
                        {
                            Id = long.Parse(SentRequests.Last().Conversation)
                        }
                    }
                };
            }
        }
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