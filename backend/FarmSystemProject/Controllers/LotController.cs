using FarmSystemProject.DTOs.FarmDTO;
using FarmSystemProject.Interfaces.IFarm;
using FarmSystemProject.Services.Interfaces.IFarm;
using Microsoft.AspNetCore.Mvc;

namespace FarmSystemProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotController : ControllerBase
{
    private readonly ILotService _lotService;
    private readonly IFarmService _farmService;
    public LotController(ILotService lotService, IFarmService farmService)
    {
        _lotService = lotService;
        _farmService = farmService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LotDTO>>> GetAll()
    {
        var lot = await _lotService.GetAll();
        return Ok(lot);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<LotDTO>> GetById(int id)
    {
        var lot = await _lotService.GetById(id);
        if (lot == null)
        {
            return NotFound($"Lote com ID {id} não encontrado.");
        }

        return Ok(lot);
    }
    [HttpPost]
    public async Task<ActionResult<LotDTO>> Create(LotDTO lotDto)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        var farm = await _farmService.GetByOwnerIdAsync(userId);
        if (lotDto.Items == null || !lotDto.Items.Any())
        {
            return BadRequest("O lote deve conter pelo menos uma raça e quantidade.");
        }

        lotDto.FarmId = farm.Id;
        var result = await _lotService.Create(lotDto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}