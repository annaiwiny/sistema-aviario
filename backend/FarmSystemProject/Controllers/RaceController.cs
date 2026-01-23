using FarmSystemProject.DTOs;
using FarmSystemProject.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RaceController : ControllerBase
{
    private readonly IRaceService _raceService;

    public RaceController(IRaceService raceService)
    {
        _raceService = raceService;
    }
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
        if (result == null) return NotFound();
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
