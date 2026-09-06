using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.NutritionalControl;
public class Feeding
{
    [Key]
    public int Id { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal ConsumptionQuantity { get; set; }

    [Required]
    public DateTime ConsumptionDate { get; set; }

    [Required]
    public int LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;
}