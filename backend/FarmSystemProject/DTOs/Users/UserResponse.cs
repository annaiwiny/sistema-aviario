namespace FarmSystemProject.DTOs.Users;

public class UserResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Cpf { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string State { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Phone { get; set; } = null!;
}