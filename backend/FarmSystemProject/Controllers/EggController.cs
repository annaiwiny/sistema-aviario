using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class EggController : ControllerBase
{
    private readonly IEggService _eggService;
    public EggController(IEggService eggService)
    {
        _eggService = eggService;
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

        var totalOvos = results.Sum(e => e.CollectQuantity);
        return Ok(new
        {
            Data = collectDate.ToShortDateString(),
            TotalColetado = totalOvos
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
}
