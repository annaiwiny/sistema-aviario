using FarmSystemProject.Configuration;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Models.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FarmSystemProject.Services;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwt;

    public JwtTokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwt = jwtOptions.Value;
    }

    public string GenerateToken(User user, out DateTime expiration)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        expiration = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Type.ToString())
        };


        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
