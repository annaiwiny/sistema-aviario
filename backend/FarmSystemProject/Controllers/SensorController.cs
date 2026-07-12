using FarmSystemProject.DTOs.Sensors;
using FarmSystemProject.Interfaces.IReportService;
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
    private readonly ISensorReportService _sensorReportService;
    private readonly IConfiguration _configuration;

    public SensorController(ISensorService sensorService, ISensorReportService sensorReportService, IConfiguration configuration)
    {
        _sensorService = sensorService;
        _sensorReportService = sensorReportService;
        _configuration = configuration;
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
        var expectedSecretKey = _configuration["Esp32Config:SecretKey"];

        if (string.IsNullOrEmpty(secretKey) || secretKey != expectedSecretKey)
        {
            throw new UnauthorizedException("Chave secreta inválida ou ausente.");
        }

        await _sensorService.RegisterEsp32Readings(payload);
        return Ok();
    }

    [HttpPost("/api/sensors")]
    [AllowAnonymous]
    public async Task<ActionResult<Sensor>> Create([FromBody] CreateSensor request, [FromHeader(Name = "X-Secret-Key")] string? secretKey)
    {
        var extractedSecretKey = Request.Headers["X-Secret-Key"].ToString();
        var expectedSecretKey = _configuration["Esp32Config:SecretKey"];

        if (string.IsNullOrEmpty(secretKey) || secretKey != expectedSecretKey)
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