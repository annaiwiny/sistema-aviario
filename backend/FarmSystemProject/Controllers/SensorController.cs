using FarmSystemProject.DTOs.Sensors;
using FarmSystemProject.Interfaces.ISensors;
using FarmSystemProject.Models.Sensors;
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
    private readonly IConfiguration _configuration;

    public SensorController(ISensorService sensorService, IConfiguration configuration)
    {
        _sensorService = sensorService;
        _configuration = configuration;
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
        var extractedSecretKey = Request.Headers["X-Secret-Key"].ToString();
        var expectedSecretKey = _configuration["Esp32Config:SecretKey"];

        if (string.IsNullOrEmpty(extractedSecretKey) || extractedSecretKey != expectedSecretKey)
        {
            throw new UnauthorizedException("Chave secreta inválida ou ausente.");
        }

        await _sensorService.RegisterEsp32Readings(payload);
        return Ok();
    }

    [HttpPost("/api/sensors")]
    [AllowAnonymous]
    public async Task<ActionResult<Sensor>> Create([FromBody] CreateSensor request)
    {
        var extractedSecretKey = Request.Headers["X-Secret-Key"].ToString();
        var expectedSecretKey = _configuration["Esp32Config:SecretKey"];

        if (string.IsNullOrEmpty(extractedSecretKey) || extractedSecretKey != expectedSecretKey)
        {
            throw new UnauthorizedException("Chave secreta inválida ou ausente.");
        }

        var createdSensor = await _sensorService.Create(request);
        return StatusCode(201, createdSensor);
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}