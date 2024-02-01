using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class SendMessageResponse
{
    [JsonProperty("ok")]
    public bool Success { get; set; }
    [JsonProperty("result")]
    public Result? Result { get; set; }
}
