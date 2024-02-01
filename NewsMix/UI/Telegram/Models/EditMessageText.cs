using NewsMix.UI.Telegram.Models;
using Newtonsoft.Json;

public class EditMessageText
{
    [JsonProperty("message_id")]
    public long MessageId { get; set; }
    [JsonProperty("chat_id")]
    public long ChatId { get; set; }
    [JsonProperty("text")]
    public string? Text { get; set; }
    [JsonProperty("reply_markup")]
    public InlineKeyboard? Keyboard { get; set; }
}