using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(255, ErrorMessage = "E-mail deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = null!;
}
