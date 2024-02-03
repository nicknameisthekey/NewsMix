using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entities;

public class Subscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int InternalUserId { get; set; }
    [MaxLength(64)] public string Source { get; set; } = null!;
    [MaxLength(64)] public string TopicInternalName { get; set; } = null!;
    public DateTime CreatedOnUTC { get; set; }
    
    [ForeignKey(nameof(InternalUserId))]
    public virtual User User { get; set; }

    public Subscription() { }
    public Subscription(string source, string topicInternalName)
    {
        Source = source;
        TopicInternalName = topicInternalName;
        CreatedOnUTC = DateTime.UtcNow;
    }

    public bool SameAs(Subscription other) => Source == other.Source && TopicInternalName == other.TopicInternalName;

    public override string ToString()
    {
        return $"Source {Source}, Topic {TopicInternalName}";
    }
}