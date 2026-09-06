using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Users;

public class UpdateUserRequest
{
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(255, ErrorMessage = "E-mail deve ter no máximo 255 caracteres")]
    public string? Email { get; set; }

    [MinLength(11, ErrorMessage = "CPF inválido")]
    [MaxLength(14, ErrorMessage = "CPF inválido")]
    public string? Cpf { get; set; }

    [MaxLength(255, ErrorMessage = "Estado deve ter no máximo 255 caracteres")]
    public string? State { get; set; }

    [MaxLength(255, ErrorMessage = "Cidade deve ter no máximo 255 caracteres")]
    public string? City { get; set; }

    [MaxLength(20, ErrorMessage = "Telefone inválido")]
    public string? Phone { get; set; }
}
