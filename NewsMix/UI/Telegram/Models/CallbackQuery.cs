using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class CallbackQuery
{
    [JsonProperty("from")]
    public User Sender { get; set; } = null!;
    [JsonProperty("data")]
    public string CallbackData { get; set; } = null!;

}
