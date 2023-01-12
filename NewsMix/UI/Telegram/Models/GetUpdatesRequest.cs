using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class GetUpdatesRequest
{
    [JsonProperty("offset")]
    public long? Offset { get; set; }
    public long? Limit { get; set; }
    public long? TimeOut { get; set; }
}
