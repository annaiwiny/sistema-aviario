using FarmSystemProject.DTOs.Auth;

namespace FarmSystemProject.Interfaces;
public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
}

