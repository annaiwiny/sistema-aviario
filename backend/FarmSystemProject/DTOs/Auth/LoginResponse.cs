using FarmSystemProject.DTOs.Users;

namespace FarmSystemProject.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public UserResponse User { get; set; } = null!;
}
