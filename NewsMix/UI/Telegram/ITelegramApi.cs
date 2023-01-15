using NewsMix.UI.Telegram.Models;

public interface ITelegramApi
{
    IAsyncEnumerable<Update> GetUpdates(CancellationToken ct);
    Task<SendMessageResponse> SendMessage(SendMessageRequest message);
    Task<SendMessageResponse> EditMessage(EditMessageText message);
}