using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.Users;

public class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [MinLength(8)]
    [MaxLength(255)]
    public string Password { get; set; } = null!;

    [MinLength(11)]
    [MaxLength(11)]
    public string Cpf { get; set; } = null!;

    public UserType Type { get; set; }

    [MaxLength(30)]
    public string? State { get; set; }

    [MaxLength(40)]
    public string? City { get; set; }

    [MaxLength(40)]
    public string? Address { get; set; }

    [MaxLength(15)]
    public string? Phone { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }
    public bool PasswordResetTokenUsed { get; set; }
}