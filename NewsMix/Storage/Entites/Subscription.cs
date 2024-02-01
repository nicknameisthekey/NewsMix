using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entites;
public class Subscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [MaxLength(64)] public string Source { get; set; } = null!;
    [MaxLength(64)] public string Topic { get; set; } = null!;

    public Subscription() { }
    public Subscription(string source, string topic)
    {
        Source = source;
        Topic = topic;
    }

    public bool SameAs(Subscription other) => Source == other.Source && Topic == other.Topic;

    public override string ToString()
    {
        return $"Source {Source}, Topic {Topic}";
    }
}