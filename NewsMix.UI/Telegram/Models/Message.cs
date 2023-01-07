using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class Message
{
    [JsonProperty("message_id")]
    public long Id { get; set; }
    [JsonProperty("text")]
    public string Text { get; set; }
    [JsonProperty("from")]
    public User Sender { get; set; }
    [JsonProperty("chat")]
    public Chat Chat { get; set; }
    [JsonProperty("date")]
    public long Date_Unix { get; set; }
    public bool IsChat => Chat.Type == "private";
    public DateTime Date => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        .AddSeconds(Date_Unix).ToLocalTime();
}