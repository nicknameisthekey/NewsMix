using System.ComponentModel.DataAnnotations;
using NewsMix.UI.Telegram;

namespace NewsMix.Storage.Entities;

public class ActiveInlineQuery
{
    [Key]
    public string QueryID { get; init; } = null!;
    public string ExternalUserId { get; set; } = null!;
    public CallbackActionType CallbackActionType { get; init; }
    public string Source { get; init; } = null!;
    public string TopicInternalName { get; init; } = null!;
}