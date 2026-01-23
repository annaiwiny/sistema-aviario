using FarmSystemProject.Models.Farm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.NutritionalControl;
public class Feeding
{
    [Key]
    public int Id { get; set; }
    [Required]
    public double ConsumptionQuantity { get; set; }
    [Required]
    public DateTime ConsumptionDate { get; set; }
    [Required]
    public int LotId { get; set; }
    [ForeignKey("LotId")]
    public  Lot Lot { get; set; }
}
