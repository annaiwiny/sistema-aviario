using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.Users;

public class User
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    [Required]
    public string Email { get; set; } = null!;

    [MinLength(8)]
    [MaxLength(255)]
    [Required]
    public string Password { get; set; } = null!;

    [MinLength(11)]
    [MaxLength(14)]
    [Required]
    public string Cpf { get; set; } = null!;

    [Required]
    public UserType Type { get; set; }

    [MaxLength(255)]
    [Required]
    public string State { get; set; } = null!;

    [MaxLength(255)]
    [Required]
    public string City { get; set; } = null!;

    [MaxLength(20)]
    [Required]
    public string Phone { get; set; } = null!;

    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }
    public bool PasswordResetTokenUsed { get; set; }
}