using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Users;

public class CreateUserRequest
{
    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    [MaxLength(255, ErrorMessage = "Senha deve ter no máximo 255 caracteres")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "CPF é obrigatório")]
    [MinLength(11, ErrorMessage = "CPF inválido")]
    [MaxLength(14, ErrorMessage = "CPF inválido")]
    public string Cpf { get; set; } = null!;

    [Required(ErrorMessage = "Estado é obrigatório")]
    [MaxLength(255, ErrorMessage = "Estado deve ter no máximo 255 caracteres")]
    public string State { get; set; } = null!;

    [Required(ErrorMessage = "Cidade é obrigatório")]
    [MaxLength(255, ErrorMessage = "Cidade deve ter no máximo 255 caracteres")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [MaxLength(20, ErrorMessage = "Telefone inválido")]
    public string Phone { get; set; } = null!;
}
