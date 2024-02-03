using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entities;

public class NotificationTask
{
    [Key]
    public string Id { get; set; }
    public int InternalUserId { get; set; }
    public string Url { get; init; } = null!;
    public string Topic { get; init; } = null!;
    public string? HashTag { get; set; }
    public bool Done { get; set; }
    public DateTime CreatedAtUTC { get; set; }
    
    [ForeignKey(nameof(InternalUserId))]
    public virtual User User { get; set; }
}