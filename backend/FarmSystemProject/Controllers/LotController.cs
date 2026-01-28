using FarmSystemProject.DTOs.Lots;
using FarmSystemProject.Interfaces.ILots;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class LotController : ControllerBase
{
    private readonly ILotService _lotService;

    public LotController(ILotService lotService)
    {
        _lotService = lotService;
    }

    [HttpPost]
    public async Task<ActionResult<LotResponse>> Create([FromBody] CreateLotRequest request)
    {
        var userId = GetUserIdFromToken();
        var response = await _lotService.Create(userId, request);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LotResponse>> GetById(int id)
    {
        var userId = GetUserIdFromToken();
        var response = await _lotService.GetById(id, userId);
        return Ok(response);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<LotResponse>> Update(int id, [FromBody] UpdateLotRequest request)
    {
        var userId = GetUserIdFromToken();
        var response = await _lotService.UpdateAsync(id, userId, request);
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