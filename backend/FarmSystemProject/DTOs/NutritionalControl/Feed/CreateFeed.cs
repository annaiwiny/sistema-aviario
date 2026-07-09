using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.NutritionalControl.Feed;

public class CreateFeed
{
    [Required(ErrorMessage = "A data da compra é obrigatória.")]
    public DateTime PurchaseDate { get; set; }

    [Required(ErrorMessage = "O peso por saco é obrigatório.")]
    [Range(typeof(decimal), "0.01", "2147483647", ErrorMessage = "O peso por saco deve ser maior que zero.")]
    public decimal BagWeight { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int BagQuantity { get; set; }

    [Required(ErrorMessage = "O valor por saco é obrigatório.")]
    [Range(typeof(decimal), "0.01", "2147483647", ErrorMessage = "O valor por saco deve ser maior que zero.")]
    public decimal BagValue { get; set; }
}
