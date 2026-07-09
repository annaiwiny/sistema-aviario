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
}