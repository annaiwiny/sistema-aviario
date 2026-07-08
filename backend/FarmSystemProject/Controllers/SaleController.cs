using FarmSystemProject.DTOs.Sales;
using FarmSystemProject.Interfaces.IReportService;
using FarmSystemProject.Interfaces.ISales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

[Authorize]
[Route("api/lots/{lotId}/sales")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly ISaleService _service;
    private readonly ISaleReportService _reportService;

    public SaleController(ISaleService service, ISaleReportService reportService)
    {
        _service = service;
        _reportService = reportService;
    }

    [HttpPost]
    public async Task<ActionResult<SaleRecordResponse>> Create(int lotId, [FromBody] CreateSaleRecord request)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.Create(lotId, userId, request);
        return CreatedAtAction(nameof(GetAll), new { lotId }, response);
    }

    // Como usar: /api/lots/1/sales/summary?date=2026-05-20
    [HttpGet("summary")]
    public async Task<ActionResult<SaleRecordSummary>> GetSummary(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var summary = await _service.GetSummaryByDate(lotId, userId, date);
        return Ok(summary);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleRecordResponse>>> GetAll(int lotId)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.GetAllByLotId(lotId, userId);
        return Ok(response);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadReport(int lotId)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateSalesListReport(lotId, userId);
        return File(fileBytes, "application/pdf", $"Vendas_Lote_{lotId}.pdf");
    }

    // Como usar: /api/lots/1/sales/pdf/daily?date=2026-05-20
    [HttpGet("pdf/daily")]
    public async Task<IActionResult> DownloadDailyReport(int lotId, [FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateSalesDateReport(lotId, userId, date);
        return File(fileBytes, "application/pdf", $"Vendas_{date:yyyyMMdd}.pdf");
    }

    private int GetUserIdFromToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Token inválido");

        return int.Parse(userId);
    }
}