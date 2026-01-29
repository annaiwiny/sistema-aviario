using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Auth;
using FarmSystemProject.DTOs.Users;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace FarmSystemProject.Services;


public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, ITokenService tokenService, IEmailService emailService, IConfiguration configuration)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
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
                Email = user.Email,
                Cpf = user.Cpf,
                Type = user.Type.ToString(),
                State = user.State,
                City = user.City,
                Phone = user.Phone
            }
        };
    }

    public async Task ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return;

        // gera token para troca de senha
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        user.PasswordResetToken = token;
        user.PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(30);
        user.PasswordResetTokenUsed = false;

        await _context.SaveChangesAsync();

        var frontendUrl = _configuration["Frontend:BaseUrl"];
        var link = $"{frontendUrl}/reset-password?token={token}";

        await _emailService.SendEmail(
            user.Email,
            "Recuperação de senha",
            $"Clique no link para redefinir sua senha: {link}"
        );
    }

    public async Task ResetPassword(ResetPasswordRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.PasswordResetToken == request.Token &&
                u.PasswordResetTokenUsed == false &&
                u.PasswordResetTokenExpiration > DateTime.UtcNow
            );

        if (user == null)
            throw new BusinessException("Token inválido ou expirado");

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordResetTokenUsed = true;
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiration = null;

        await _context.SaveChangesAsync();
    }
}