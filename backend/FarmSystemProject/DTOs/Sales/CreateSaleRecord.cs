using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Sales;

public class CreateSaleRecord
{
    [Required(ErrorMessage = "O valor unitário é obrigatório.")]
    [Range(typeof(decimal), "0.01", "2147483647", ErrorMessage = "O valor unitário deve ser maior que zero.")]
    public decimal UnitValue { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int EggQuantity { get; set; }

    [Required(ErrorMessage = "A data de venda é obrigatória.")]
    public DateTime SaleDate { get; set; }
}