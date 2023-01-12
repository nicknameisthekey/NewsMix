using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class GetUpdatesResponse
{
    [JsonProperty("ok")]
    public bool Success { get; set; }
    [JsonProperty("result")]
    public List<Update> Updates { get; set; } = new List<Update>();
}

