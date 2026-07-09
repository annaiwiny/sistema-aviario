using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.NutricionalControl.Feeding;

public class CreateFeeding
{
    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(typeof(decimal), "0.01", "2147483647", ErrorMessage = "A quantidade deve ser maior que zero.")]
    public decimal ConsumptionQuantity { get; set; }

    [Required(ErrorMessage = "A data de consumo é obrigatória.")]
    public DateTime ConsumptionDate { get; set; }
}