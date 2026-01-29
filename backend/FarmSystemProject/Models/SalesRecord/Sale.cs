using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.SalesRecord;
public class Sale
{
    [Key]
    public int Id { get; set; }
    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal UnitValue { get; set; }
    [Required]
    public int EggQuantity { get; set; }
    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal TotalValue { get; set; }
    [Required]
    public DateTime SaleDate { get; set; }
    [Required]
    public int LotId { get; set; }
    [ForeignKey("LotId")]
    public Lot Lot { get; set; }
}
