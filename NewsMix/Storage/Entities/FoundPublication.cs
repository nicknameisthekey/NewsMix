using System.ComponentModel.DataAnnotations;

namespace NewsMix.Storage.Entities;

public class FoundPublication
{
    [Key] public string PublicationUrl { get; set; } = null!;
    public string TopicInternalName { get; set; } = null!;
    public string Source { get; set; } = null;
    public string? HashTag { get; set; }
    public DateTime CreatedAtUTC { get; set; }
}