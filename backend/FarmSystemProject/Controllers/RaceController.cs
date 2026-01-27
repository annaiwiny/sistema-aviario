using FarmSystemProject.DTOs.FarmDTO;
using FarmSystemProject.Interfaces;
using FarmSystemProject.Interfaces.IFarm;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RaceController(IRaceService raceService, IRaceReportService raceReportService) : ControllerBase
{
    private readonly IRaceService _raceService = raceService;
    private readonly IRaceReportService _raceReportService = raceReportService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RaceDTO>>> GetAll()
    {
        var races = await _raceService.GetAll();
        return Ok(races);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<RaceDTO>> GetById(int id)
    {
        var result = await _raceService.GetById(id);
        
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpPost]
    public async Task<ActionResult<RaceDTO>> Create(RaceDTO raceDto)
    {
        var result = await _raceService.Create(raceDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, RaceDTO raceDto)
    {
        if (id != raceDto.Id)
        {
            return BadRequest("O ID da URL é diferente do ID do corpo");
        }

        await _raceService.Update(id, raceDto);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _raceService.Delete(id);
        return NoContent();
    }
}
