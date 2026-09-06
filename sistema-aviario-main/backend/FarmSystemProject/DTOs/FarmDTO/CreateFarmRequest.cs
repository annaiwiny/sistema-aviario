using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.FarmDTO;

public class CreateFarmRequest
{
    [Required(ErrorMessage = "O nome do aviário é obrigatório.")]
    [MaxLength(255, ErrorMessage = "O nome do aviário deve ter no máximo 255 caracteres")]
    public string Name { get; set; } = null!;
}