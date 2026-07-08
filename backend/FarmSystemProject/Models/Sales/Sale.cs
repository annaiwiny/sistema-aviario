using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Sales;
public class Sale
{
    [Key]
    public int Id { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal UnitValue { get; set; }

    [Required]
    public int EggQuantity { get; set; }

    // Atributo auxiliar (Não cria coluna no banco)
    [NotMapped]
    public decimal TotalValue => UnitValue * EggQuantity;

    [Required]
    public DateTime SaleDate { get; set; }

    [Required]
    public int LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;
}