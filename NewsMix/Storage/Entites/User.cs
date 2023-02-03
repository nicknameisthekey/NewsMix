using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entites;

[Table("Users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    [MaxLength(64)]
    public string Name { get; set; } = null!;
    [MaxLength(64)]
    public string UIType { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<UserAction> UserActions { get; init; } = new();
    public List<Subscription> Subscriptions { get; init; } = new();
}