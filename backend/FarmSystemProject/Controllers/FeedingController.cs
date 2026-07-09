using FarmSystemProject.DTOs.NutritionalControl.Feeding;
using FarmSystemProject.Interfaces.INutritionalControl;
using FarmSystemProject.Interfaces.IReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/lots/{lotId}/feedings")]
[ApiController]
public class FeedingController : ControllerBase
{
    private readonly IFeedingService _service;
    private readonly IFeedingReportService _reportService;

    public FeedingController(IFeedingService service, IFeedingReportService reportService)
    {
        _service = service;
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<ActionResult<FeedingResponse>> Create(int lotId, [FromBody] CreateFeeding request)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.Create(lotId, userId, request);
        return CreatedAtAction(nameof(GetAll), new { lotId }, response);
    }

    // Como usar: /api/lots/1/feedings/summary?date=2026-05-20
    [HttpGet("summary")]
    public async Task<ActionResult<FeedingSummary>> GetSummary(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var summary = await _service.GetSummaryByDate(lotId, userId, date);
        return Ok(summary);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeedingResponse>>> GetAll(int lotId)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.GetAllByLotId(lotId, userId);
        return Ok(response);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadReport(int lotId)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateFeedingListReport(lotId, userId);
        return File(fileBytes, "application/pdf", $"Controle_Alimentacao_Lote_{lotId}.pdf");
    }

    // Como usar: /api/lots/1/feedings/pdf/daily?date=2026-05-20
    [HttpGet("pdf/daily")]
    public async Task<IActionResult> DownloadDailyReport(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateFeedingDateReport(lotId, userId, date);
        return File(fileBytes, "application/pdf", $"Controle_Alimentacao_{date:yyyyMMdd}.pdf");
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}