using FarmSystemProject.Data;
using FarmSystemProject.DTOs.FarmDTO;
using FarmSystemProject.Interfaces.IFarm;
using FarmSystemProject.Models.Farm;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services;
public class RaceService : IRaceService
{
    private readonly AppDbContext _context;

    public RaceService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<RaceDTO>> GetAll()
    {
        return await _context.Races.Select(r => new RaceDTO { Id = r.Id, Name = r.Name }).ToListAsync();
    }
    public async Task<RaceDTO?> GetById(int id)
    {
        var race = await _context.Races.FindAsync(id);
        
        if (race == null)
        {
            return null;
        }

        return new RaceDTO { Id = race.Id, Name = race.Name };
    }
    public async Task<RaceDTO> Create(RaceDTO raceDto)
    {
        var race = new Race { Name = raceDto.Name };
        _context.Races.Add(race);
        await _context.SaveChangesAsync();
        raceDto.Id = race.Id;
        return raceDto;
    }
    public async Task Update(int id, RaceDTO raceDto)
    {
        var race = await _context.Races.FindAsync(id);
        if (race != null)
        {
            race.Name = raceDto.Name;
            await _context.SaveChangesAsync();
        }
    }
    public async Task Delete(int id)
    {
        var race = await _context.Races.FindAsync(id);
        if (race != null)
        {
            _context.Races.Remove(race);
            await _context.SaveChangesAsync();
        }
    }
}
