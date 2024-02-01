using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class InlineKeyboardButton
{
    [JsonProperty("text")]
    public string Text { get; set; } = null!;
    [JsonProperty("callback_data")]
    public string CallBackData { get; set; } = null!;
}
