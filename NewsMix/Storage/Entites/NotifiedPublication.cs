using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entites;

public class NotifiedPublication
{
    [Key]
    public string PublicationUniqeID { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}