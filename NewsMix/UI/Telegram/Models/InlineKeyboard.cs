using Newtonsoft.Json;

namespace NewsMix.UI.Telegram.Models;
public class InlineKeyboard
{
    [JsonProperty("inline_keyboard")]
    public object[,] Keyboard { get; set; }
}

