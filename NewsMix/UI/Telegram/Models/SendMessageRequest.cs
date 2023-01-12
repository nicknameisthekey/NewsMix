using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class SendMessageRequest
{
    [JsonProperty("chat_id")]
    public string Conversation { get; set; }
    [JsonProperty("text")]
    public string? Text { get; set; }
    [JsonProperty("reply_to_message_id")]
    public long? ReplyToMessageId { get; set; }
    [JsonProperty("reply_markup")]
    public InlineKeyboard Keyboard { get; set; }
}
