using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Users;

public class UpdateUserRequest
{
    [MaxLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string? Name { get; set; }

    public DateTime? BirthDate { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(255, ErrorMessage = "E-mail deve ter no máximo 255 caracteres")]
    public string? Email { get; set; }

    [MinLength(11, ErrorMessage = "CPF deve conter exatamente 11 caracteres")]
    [MaxLength(11, ErrorMessage = "CPF deve conter exatamente 11 caracteres")]
    public string? Cpf { get; set; }

    [MaxLength(30, ErrorMessage = "Estado deve ter no máximo 30 caracteres")]
    public string? State { get; set; }

    [MaxLength(40, ErrorMessage = "Cidade deve ter no máximo 40 caracteres")]
    public string? City { get; set; }

    [MaxLength(40, ErrorMessage = "Endereço deve ter no máximo 40 caracteres")]
    public string? Address { get; set; }

    [MaxLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
    public string? Phone { get; set; }
}
