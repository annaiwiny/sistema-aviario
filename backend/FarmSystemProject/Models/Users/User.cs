using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.Users;
public class User
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required, MaxLength(14)]
    public string Cpf { get; set; }
    [Required]
    public UserType Type { get; set; }
    [MaxLength(30)]
    public string State { get; set; }
    [MaxLength(40)]
    public string City { get; set; }
    [MaxLength(40)]
    public string Address { get; set; }
    [MaxLength(15)]
    public string Phone { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime LockoutEnd { get; set; }
}
