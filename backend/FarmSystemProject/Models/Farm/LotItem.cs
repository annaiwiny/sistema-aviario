using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.Farm;
public class LotItem
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Race { get; set; } = null!;
    [Required]
    public int Quantity { get; set; }
    public int LotId { get; set; }
    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;
}
