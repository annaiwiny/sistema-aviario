using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Interfaces.IReportInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/lots/{lotId}/eggs")]
[ApiController]
public class EggProductionController : ControllerBase
{
    private readonly IEggProductionService _service;
    private readonly IEggProductionReportService _reportService;

    public EggProductionController(IEggProductionService service, IEggProductionReportService reportService)
    {
        _service = service;
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<ActionResult<EggProductionResponse>> Create(int lotId, [FromBody] CreateEggProductionRequest request)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.Create(lotId, userId, request);
        return CreatedAtAction(nameof(GetAll), new { lotId }, response);
    }

    // Como usar: /api/lots/5/eggs/summary?date=2026-05-20
    [HttpGet("summary")]
    public async Task<ActionResult<EggProductionDateSummary>> GetSummary(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var summary = await _service.GetSummaryByDate(lotId, userId, date);
        return Ok(summary);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EggProductionResponse>>> GetAll(int lotId)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.GetAllByLotId(lotId, userId);
        return Ok(response);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadReport(int lotId)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateEggListReport(lotId, userId);
        return File(fileBytes, "application/pdf", $"ProducaoOvos_Lote_{lotId}.pdf");
    }

    // Como usar: /api/lots/1/eggs/pdf/daily?date=2026-05-20
    [HttpGet("pdf/daily")]
    public async Task<IActionResult> DownloadDailyReport(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateEggDateReport(lotId, userId, date);
        return File(fileBytes, "application/pdf", $"ProducaoOvos_{date:yyyyMMdd}.pdf");
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}