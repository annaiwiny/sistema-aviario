using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MaxLength(255, ErrorMessage = "Senha deve ter no máximo 255 caracteres")]
    public string Password { get; set; } = null!;
}