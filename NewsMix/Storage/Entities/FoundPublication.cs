using System.ComponentModel.DataAnnotations;

namespace NewsMix.Storage.Entities;

public class FoundPublication
{
    [Key]
    public string PublicationUrl { get; set; } = null!;
    public DateTime CreatedAtUTC { get; set; }
}