using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Lots;

public class CreateLineageRequest
{
    [Required(ErrorMessage = "O nome da raça é obrigatório.")]
    [MaxLength(255, ErrorMessage = "O nome da raça deve ter no máximo 255 caracteres")]
    public string Race { get; set; } = string.Empty;

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantity { get; set; }
}
