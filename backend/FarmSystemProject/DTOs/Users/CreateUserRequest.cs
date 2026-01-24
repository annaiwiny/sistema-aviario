using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Users;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(255, ErrorMessage = "Nome deve ter no máximo 255 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [MaxLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(8, ErrorMessage = "Senha deve ter no mínimo 8 caracteres")]
    [MaxLength(255, ErrorMessage = "Senha deve ter no máximo 255 caracteres")]
    public string Password { get; set; }

    [Required(ErrorMessage = "CPF é obrigatório")]
    [MinLength(11, ErrorMessage = "CPF deve ter no mínimo 11 caracteres")]
    [MaxLength(11, ErrorMessage = "CPF deve ter no máximo 11 caracteres")]
    public string Cpf { get; set; }

    [MaxLength(30, ErrorMessage = "Estado deve ter no máximo 30 caracteres")]
    public string? State { get; set; }

    [MaxLength(40, ErrorMessage = "Cidade deve ter no máximo 40 caracteres")]
    public string? City { get; set; }

    [MaxLength(40, ErrorMessage = "Endereço deve ter no máximo 40 caracteres")]
    public string? Address { get; set; }

    [MaxLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
    public string? Phone { get; set; }
}
