using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class Chat
{
    [JsonProperty("id")]
    public long Id { get; set; }
    [JsonProperty("type")]
    public string? Type { get; set; }
}
