using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Interfaces.IReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/lots/{lotId}/vaccinations")]
[ApiController]
public class VaccinationController : ControllerBase
{
    private readonly IVaccinationService _service;
    private readonly IVaccinationReportService _reportService;

    public VaccinationController(IVaccinationService service, IVaccinationReportService reportService)
    {
        _service = service;
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<ActionResult<VaccinationResponse>> Create(int lotId, [FromBody] CreateVaccinationRequest request)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.Create(lotId, userId, request);
        return CreatedAtAction(nameof(GetAll), new { lotId }, response);
    }

    // Como usar: /api/lots/1/vaccinations/summary?date=2026-05-20
    [HttpGet("summary")]
    public async Task<ActionResult<VaccinationDateSummary>> GetSummary(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var summary = await _service.GetSummaryByDate(lotId, userId, date);
        return Ok(summary);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VaccinationResponse>>> GetAll(int lotId)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.GetAllByLotId(lotId, userId);
        return Ok(response);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadReport(int lotId)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateVaccinationListReport(lotId, userId);
        return File(fileBytes, "application/pdf", $"Vacinacao_Lote_{lotId}.pdf");
    }

    // Como usar: /api/lots/1/vaccinations/pdf/daily?date=2026-05-20
    [HttpGet("pdf/daily")]
    public async Task<IActionResult> DownloadDailyReport(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateVaccinationDateReport(lotId, userId, date);
        return File(fileBytes, "application/pdf", $"Vacinacao_{date:yyyyMMdd}.pdf");
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}