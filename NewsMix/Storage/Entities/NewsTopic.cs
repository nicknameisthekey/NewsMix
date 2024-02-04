using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsMix.Storage.Entities;

public class NewsTopic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string NewsSource { get; set; } = null!;
    public string InternalName { get; set; } = null!;
    public string VisibleNameRU { get; set; } = null!;
    public string? HashTag { get; set; }
    public byte OrderInList { get; set; }
    public bool Enabled { get; set; }
}