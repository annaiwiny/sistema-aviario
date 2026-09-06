using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Auth;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Token é obrigatório")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres")]
    [MaxLength(255, ErrorMessage = "Senha deve ter no máximo 255 caracteres")]
    public string NewPassword { get; set; } = null!;
}
