using NewsMix.UI.Telegram.Models;

public interface ITelegramApi
{
    Task<List<Update>> GetUpdates();
    Task<SendMessageResponse> SendMessage(SendMessageRequest message);
    Task<SendMessageResponse> EditMessage(EditMessageText message);
}