using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class User
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("username")]
    public string? UserName { get; set; }
}
