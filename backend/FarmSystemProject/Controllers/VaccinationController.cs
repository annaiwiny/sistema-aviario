using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;
[ApiController]
[Route("api/[Controller]")]
public class VaccinationController : ControllerBase
{
    private readonly IVaccinationService _vaccinationService;
    public VaccinationController(IVaccinationService vaccinationService)
    {
        _vaccinationService = vaccinationService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VaccinationDTO>>> GetAll()
    {
        var vaccination = await _vaccinationService.GetAll();
        return Ok(vaccination);
    }
    [HttpGet("{applicationDate}")]
    public async Task<ActionResult<VaccinationDTO>> GetByDate(DateTime applicationDate)
    {
        var results = await _vaccinationService.GetByDate(applicationDate);
        if (results == null || !results.Any())
        {
            return NotFound("Nenhum registro encontrado para essa data.");
        }

        var totalApplication = results.Sum(v => v.ApplicationQuantity);
        var totalValue = results.Sum(v => v.ApplicationValue);

        return Ok( new 
        {
            Data = applicationDate.ToShortDateString(),
            QuantidadeAplicacoes = totalApplication,
            ValorTotal = totalValue
        });
    }
    [HttpPost]
    public async Task<ActionResult<VaccinationDTO>> Create(VaccinationDTO vaccinationDto)
    {
        var result = await _vaccinationService.Create(vaccinationDto);
        return CreatedAtAction(
            nameof(GetByDate),
            new { applicationDate = result.ApplicationDate.ToString("yyyy-MM-dd") },
            result
        );
    }
}
