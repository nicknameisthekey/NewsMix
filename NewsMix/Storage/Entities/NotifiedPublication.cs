using System.ComponentModel.DataAnnotations;

namespace NewsMix.Storage.Entities;

public class NotifiedPublication
{
    [Key]
    public string PublicationUniqeID { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}