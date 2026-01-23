using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.NutritionalControl;
public class Feed
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime PurchaseDate { get; set; }
    [Required]
    public double BagWeight { get; set; }
    [Required]
    public int BagQuantity { get; set; }
    [Required]
    public decimal BagValue { get; set; }
}
