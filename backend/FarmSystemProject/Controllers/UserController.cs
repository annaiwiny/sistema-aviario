using FarmSystemProject.DTOs.Users;
using FarmSystemProject.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var user = await _userService.Create(request);
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Read()
    {
        var user = await _userService.Read(GetUserIdFromToken());
        return Ok(user);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        var user = await _userService.Update(GetUserIdFromToken(), request);
        return Ok(user);
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}
