using NewsMix.UI.Telegram.Models;
using System.Linq;
public class ButtonPress
{
    public (int Row, int Col) ButtonNumber { get; init; }
    public string? ButtonWithText { get; init; }

    public ButtonPress(string buttonWithText)
    {
        ButtonWithText = buttonWithText;
    }

    public ButtonPress(int Row, int Col) => ButtonNumber = (Row, Col);
}

public class FakeTelegramApi : ITelegramApi
{
    private readonly List<object> _updates = new();

    public List<SendMessageRequest> SentRequests = new();
    public List<EditMessageText> EditedMessages = new();
    private int currentResultMessageId = 0;

    public FakeTelegramApi AddUpdate(object update)
    {
        _updates.Add(update);
        return this;
    }

    public Task<SendMessageResponse> EditMessage(EditMessageText message)
    {
        EditedMessages.Add(message);
        currentResultMessageId++;
        return Task.FromResult(new SendMessageResponse
        {
            Result = new Result
            {
                MessageId = currentResultMessageId
            },
            Success = true
        });
    }

    public async IAsyncEnumerable<Update> GetUpdates(CancellationToken ct)
    {
        foreach (var update in _updates)
        {
            if (update is Update u)
            {
                yield return u;
            }
            else if (update is ButtonPress cu)
            {
                InlineKeyboardButton? button = null;
                if (cu.ButtonWithText == null)
                {
                    button = SentRequests.Last().Keyboard
                       .Keyboard[cu.ButtonNumber.Col, cu.ButtonNumber.Row] as InlineKeyboardButton;
                }
                else
                {
                    button = SentRequests.Last().Keyboard.Keyboard
                    .Cast<InlineKeyboardButton>()
                    .First(k => k.Text.Contains(cu.ButtonWithText));
                }

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

    public Task<SendMessageResponse> SendMessage(SendMessageRequest message)
    {
        SentRequests.Add(message);
        currentResultMessageId++;
        return Task.FromResult(new SendMessageResponse
        {
            Result = new Result
            {
                MessageId = currentResultMessageId
            },
            Success = true
        });
    }
}