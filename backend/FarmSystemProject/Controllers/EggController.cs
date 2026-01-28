using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class EggController : ControllerBase
{
    private readonly IEggService _eggService;
    private readonly IEggReportService _eggReportService;
    public EggController(IEggService eggService, IEggReportService eggReportService)
    {
        _eggService = eggService;
        _eggReportService = eggReportService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EggDTO>>> GetAll()
    {
        var eggs = await _eggService.GetAll();
        return Ok(eggs);
    }
    [HttpGet("{collectDate}")]
    public async Task<ActionResult<EggDTO>> GetByDate(DateTime collectDate)
    {
        var results = await _eggService.GetByDate(collectDate);
        if (results == null || !results.Any())
        {
            return NotFound("Nenhuma coleta encontrada para esta data.");
        }

        var totalEggs = results.Sum(e => e.CollectQuantity);
        return Ok(new
        {
            Data = collectDate.ToShortDateString(),
            TotalColetado = totalEggs
        });
    }
    [HttpPost]
    public async Task<ActionResult<EggDTO>> Create(EggDTO eggDto)
    {
        var result = await _eggService.Create(eggDto);
        return CreatedAtAction(
            nameof(GetByDate),
            new { collectDate = result.CollectDate.ToString("yyyy-MM-dd") },
            result
        );
    }
    [HttpGet("report")]
    public async Task<IActionResult> DownloadReport()
    {
        var pdf = await _eggReportService.GenerateEggListReport();
        return File(pdf, "application/pdf", "Relatorio_Produção_de_Ovos.pdf");
    }
    [HttpGet("report/{collectDate}")]
    public async Task<IActionResult> DownloadReportDate(DateTime collectDate)
    {
        var pdf = await _eggReportService.GenerateEggDateReport(collectDate);
        return File(pdf, "application/pdf", "Relatorio_Produção_de_Ovos.pdf");
    }
}
