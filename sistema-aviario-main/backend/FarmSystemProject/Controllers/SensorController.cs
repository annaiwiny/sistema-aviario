using FarmSystemProject.Configuration;
using FarmSystemProject.DTOs.Sensors;
using FarmSystemProject.Interfaces.IReportService;
using FarmSystemProject.Interfaces.ISensors;
using FarmSystemProject.Models.Sensors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/lots/{lotId}/sensors")]
[ApiController]
public class SensorController : ControllerBase
{
    private readonly ISensorService _sensorService;
    private readonly ISensorReportService _sensorReportService;
    private readonly Esp32Options _esp32;

    public SensorController(ISensorService sensorService, ISensorReportService sensorReportService, IOptions<Esp32Options> esp32Options)
    {
        _sensorService = sensorService;
        _sensorReportService = sensorReportService;
        _esp32 = esp32Options.Value;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<List<SensorSummary>>> GetSensorsSummary(int lotId)
    {
        var userId = GetUserIdFromToken();
        var response = await _sensorService.GetSensorsSummary(lotId, userId);
        return Ok(response);
    }

    [HttpGet("report/{type}")]
    public async Task<IActionResult> GetSensorMonitoringReport(int lotId, SensorType type)
    {
        var userId = GetUserIdFromToken();
        var pdfBytes = await _sensorReportService.GenerateSensorMonitoringReport(lotId, userId, type);
        return File(pdfBytes, "application/pdf", $"relatorio_monitoramento_{type}_{lotId}.pdf");
    }

    [HttpPost("/api/sensors/readings")]
    [AllowAnonymous]
    public async Task<IActionResult> ReceiveReadings([FromBody] Esp32Payload payload, [FromHeader(Name = "X-Secret-Key")] string? secretKey)
    {
        EnsureValidSecretKey(secretKey);

        await _sensorService.RegisterEsp32Readings(payload);
        return Ok();
    }

    [HttpPost("/api/sensors")]
    [AllowAnonymous]
    public async Task<ActionResult<Sensor>> Create([FromBody] CreateSensor request, [FromHeader(Name = "X-Secret-Key")] string? secretKey)
    {
        EnsureValidSecretKey(secretKey);

        var createdSensor = await _sensorService.Create(request);
        return StatusCode(201, createdSensor);
    }

    /// <summary>
    /// Compara a chave enviada pelo ESP32 em tempo constante, para não vazar
    /// o prefixo correto da chave através do tempo de resposta.
    /// </summary>
    private void EnsureValidSecretKey(string? secretKey)
    {
        if (string.IsNullOrEmpty(secretKey))
            throw new UnauthorizedException("Chave secreta inválida ou ausente.");

        var provided = Encoding.UTF8.GetBytes(secretKey);
        var expected = Encoding.UTF8.GetBytes(_esp32.SecretKey);

        if (!CryptographicOperations.FixedTimeEquals(provided, expected))
            throw new UnauthorizedException("Chave secreta inválida ou ausente.");
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}