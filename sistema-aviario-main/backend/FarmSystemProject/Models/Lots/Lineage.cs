using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.Lots;
public class Lineage
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Race { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    public int LotId { get; set; }
    [ForeignKey("LotId")]

    public Lot Lot { get; set; } = null!;
}