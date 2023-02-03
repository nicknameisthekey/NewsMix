using Newtonsoft.Json;
namespace NewsMix.UI.Telegram.Models;
public class Update
{
    [JsonProperty("update_id")]
    public long Id { get; set; }
    [JsonProperty("message")]
    public Message? Message { get; set; }
    [JsonProperty("callback_query")]
    public CallbackQuery? CallBack { get; set; }

    [JsonIgnore]
    public bool IsCallback => CallBack?.CallbackData != null;

    [JsonIgnore]
    public bool HasTextMessage => Message?.Text != null;

    [JsonIgnore]
    public string UserId => (CallBack?.Sender?.Id ??
                             Message?.Chat?.Id ??
                             Message!.Sender!.Id)!.ToString();

    [JsonIgnore]
    public string UserName => Message?.Chat?.UserName ??
                              Message?.Sender?.UserName ??
                              CallBack?.Sender.UserName ?? "unkown";

    public bool OlderThan(int minutes) => Message?.Date < DateTime.Now.AddMinutes(-minutes);
}
