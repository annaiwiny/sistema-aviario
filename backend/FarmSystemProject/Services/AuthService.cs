using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Auth;
using FarmSystemProject.DTOs.Users;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new BusinessException("E-mail ou senha inválidos");

        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            throw new BusinessException("Conta bloqueada por 15 minutos devido a múltiplas tentativas de login");

        var passwordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.Password
        );

        if (!passwordValid)
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= 3)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                user.FailedLoginAttempts = 0;
            }
            await _context.SaveChangesAsync();

            throw new BusinessException("E-mail ou senha inválidos");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        await _context.SaveChangesAsync();

        var token = _tokenService.GenerateToken(user, out var expiration);

        return new LoginResponse
        {
            Token = token,
            Expiration = expiration,
            User = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Type = user.Type.ToString()
            }
        };
    }
}