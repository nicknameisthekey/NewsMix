using Newtonsoft.Json;
namespace NewsMix.UI.Telegram.Models;
public class Update
{
    [JsonProperty("update_id")]
    public long Id { get; set; }
    [JsonProperty("message")]
    public Message Message { get; set; }
    [JsonProperty("callback_query")]
    public CallbackQuery CallBack { get; set; }
}
