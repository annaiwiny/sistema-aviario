using FarmSystemProject.Models.Users;

namespace FarmSystemProject.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user, out DateTime expiration);
}

