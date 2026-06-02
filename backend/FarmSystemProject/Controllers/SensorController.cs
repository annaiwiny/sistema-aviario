using FarmSystemProject.DTOs.Sensors;
using FarmSystemProject.Interfaces.ISensors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/lots/{lotId}/sensors")]
[ApiController]
public class SensorController : ControllerBase
{
    private readonly ISensorService _sensorService;

    public SensorController(ISensorService sensorService)
    {
        _sensorService = sensorService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<SensorSummary>>> GetSensorsSummary(int lotId)
    {
        var userId = GetUserIdFromToken();
        var response = await _sensorService.GetSensorsSummary(lotId, userId);
        return Ok(response);
    }

    [HttpPost("/api/sensors/readings")]
    [AllowAnonymous]
    public async Task<IActionResult> ReceiveReadings([FromBody] Esp32Payload payload)
    {
        await _sensorService.RegisterEsp32Readings(payload);
        return Ok();
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}