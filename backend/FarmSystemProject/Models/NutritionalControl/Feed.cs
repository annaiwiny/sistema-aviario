using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.NutritionalControl;
public class Feed
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime PurchaseDate { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal BagWeight { get; set; }

    [Required]
    public int BagQuantity { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal BagValue { get; set; }

    // Atributo auxiliar (Não cria coluna no banco)
    [NotMapped]
    public decimal TotalCost => BagQuantity * BagValue;

    [Required]
    public int LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;
}