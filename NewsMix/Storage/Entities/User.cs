using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entities;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string ExternalUserId { get; set; } = null!;
    [MaxLength(64)]
    public string Name { get; set; } = null!;
    [MaxLength(64)]
    public string UIType { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    
    public List<UserAction>? UserActions { get; set; }
    public List<Subscription>? Subscriptions { get; set; }
    public List<NotificationTask>? NotificationTasks { get; set; }
}