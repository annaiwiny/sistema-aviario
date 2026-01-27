using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;
[ApiController]
[Route("api/[Controller]")]
public class MortalityController : ControllerBase
{
    private readonly IMortalityService _mortalityService;
    private readonly IMortalityReportService _mortalityReportService; // Corrigido o tipo

    public MortalityController(IMortalityService mortalityService, IMortalityReportService mortalityReportService) // Corrigido o construtor
    {
        _mortalityService = mortalityService;
        _mortalityReportService = mortalityReportService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable< MortalityDTO>>> GetAll()
    {
        var mortality = await _mortalityService.GetAll();
        return Ok(mortality);
    }
    [HttpGet("{dateDeath}")]
    public async Task<ActionResult<MortalityDTO>> GetByDate(DateTime dateDeath)
    {
        var results = await _mortalityService.GetByDate(dateDeath);
        if (results == null || !results.Any())
        {
            return NotFound("Nenhuma morte encontrada para esta data.");
        }

        var totalDeath = results.Sum(m => m.DeathQuantity);
        return Ok(new
        {
            Data = dateDeath.ToShortDateString(),
            TotalMortes = totalDeath
        });
    }
    [HttpPost]
    public async Task<ActionResult<MortalityDTO>> Create(MortalityDTO mortalityDto)
    {
        var result = await _mortalityService.Create(mortalityDto);
        return CreatedAtAction(
            nameof(GetByDate),
            new { dateDeath = result.DateDeath.ToString("yyyy-MM-dd") },
            result
        );
    }
    // Começa aqui a rota do relatório
    [HttpGet("report")]
    public async Task<IActionResult> DownloadReport()
    {
        var pdf = await _mortalityReportService.GenerateMortalityListReport();
        return File(pdf, "application/pdf", "Relatorio_Mortes.pdf");
    }
}
