using FarmSystemProject.Models.Farm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.ProductiveMonitoring;
public class CollectEgg
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime CollectDate { get; set; }
    [Required]
    public int CollectQuantity { get; set; }
    [Required]
    public int LotId { get; set; }
    [ForeignKey("LotId")]
    public Lot Lot { get; set; }
}
