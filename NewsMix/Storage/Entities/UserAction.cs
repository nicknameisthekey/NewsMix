using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entities;

public class UserAction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int InternalUserId { get; set; }
    public ActionType ActionType { get; set; }
    [MaxLength(64)] public string Source { get; set; } = null!;
    [MaxLength(64)] public string Topic { get; set; } = null!;
    public DateTime CreatedAtUTC { get; set; }
    
    [ForeignKey(nameof(InternalUserId))]
    public virtual User User {get; set; }
}

public enum ActionType
{
    Subscribe = 1,
    Unsubscribe = 2
}