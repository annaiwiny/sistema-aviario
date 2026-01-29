using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.ProductiveMonitoring;
public class EggProduction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime ProductionDate { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public int LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;
}