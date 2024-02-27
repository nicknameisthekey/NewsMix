using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entities;

public class NotificationTask
{
    [Key]
    public int Id { get; set; }
    public int InternalUserId { get; set; }
    public string Url { get; set; } = null!;
    public string NewsSource { get; set; } = null!;
    public string TopicInternalName { get; set; } = null!;
    public string? HashTag { get; set; }
    public DateTime? DoneAtUTC { get; set; }
    public DateTime CreatedAtUTC { get; set; }
    
    [ForeignKey(nameof(InternalUserId))]
    public virtual User User { get; set; }
}