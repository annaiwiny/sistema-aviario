using FarmSystemProject.DTOs.Sales;
using FarmSystemProject.Interfaces.IReportService;
using FarmSystemProject.Interfaces.ISales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmSystemProject.Controllers;

// Venda é da granja inteira, não de um lote: os ovos de todos os lotes são
// juntados antes de vender. Por isso a rota é /api/farm/sales e não
// /api/lots/{lotId}/sales, como era antes.
[Authorize]
[Route("api/farm/sales")]
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
    public async Task<ActionResult<SaleRecordResponse>> Create([FromBody] CreateSaleRecord request)
    {
        var userId = GetUserIdFromToken();
        var response = await _service.Create(userId, request);
        return CreatedAtAction(nameof(GetAll), new { }, response);
    }

    // Como usar: /api/farm/sales/summary?date=2026-05-20
    [HttpGet("summary")]
    public async Task<ActionResult<SaleRecordSummary>> GetSummary([FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var summary = await _service.GetSummaryByDate(userId, date);
        return Ok(summary);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleRecordResponse>>> GetAll()
    {
        var userId = GetUserIdFromToken();
        var response = await _service.GetAllByFarm(userId);
        return Ok(response);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadReport()
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateSalesListReport(userId);
        return File(fileBytes, "application/pdf", "Vendas_Granja.pdf");
    }

    // Como usar: /api/farm/sales/pdf/daily?date=2026-05-20
    [HttpGet("pdf/daily")]
    public async Task<IActionResult> DownloadDailyReport([FromQuery] DateTime date)
    {
        var userId = GetUserIdFromToken();
        var fileBytes = await _reportService.GenerateSalesDateReport(userId, date);
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
