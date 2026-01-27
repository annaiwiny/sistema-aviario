using FarmSystemProject.DTOs.FarmDTO;
using FarmSystemProject.Services.Interfaces.IFarm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FarmController : ControllerBase
{
    private readonly IFarmService _farmService;

    public FarmController(IFarmService farmService)
    {
        _farmService = farmService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateFarmRequest request)
    {
        var userId = GetUserIdFromToken();
        var response = await _farmService.CreateAsync(userId, request);

        return CreatedAtAction(nameof(GetMyFarm), new { }, response);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMyFarm()
    {
        var userId = GetUserIdFromToken();
        var response = await _farmService.GetByOwnerIdAsync(userId);

        return Ok(response);
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}