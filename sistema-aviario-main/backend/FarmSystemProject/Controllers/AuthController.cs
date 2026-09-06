using FarmSystemProject.DTOs.Auth;
using FarmSystemProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.Login(request);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPassword(request);
        return Ok(new{message = "Se o e-mail estiver cadastrado, você receberá um link de recuperação."});
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPassword(request);
        return Ok(new{message = "Senha redefinida com sucesso."});
    }
}