using Newtonsoft.Json;
namespace NewsMix.UI.Telegram.Models;

public class InlineKeyboardMarkup
{
    [JsonProperty("inline_keyboard")]
    public InlineKeyboardButton[,] Buttons { get; set; } = null!;
}