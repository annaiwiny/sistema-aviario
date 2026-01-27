using FarmSystemProject.Data;
using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Models.HealthMonitoring;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.HelthMonitoringService;
public class MortalityService : IMortalityService
{
    private readonly AppDbContext _context;
    public MortalityService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<MortalityDTO>> GetAll()
    {
        return await _context.Mortalities.Select(m => new MortalityDTO
        {
            Id = m.Id,
            DateDeath = m.DateDeath,
            DeathQuantity = m.DeathQuantity,
            CutQuantity = m.CutQuantity,
            Reason = m.Reason,
            LotId = m.LotId
        }).ToListAsync();
    }
    public async Task<IEnumerable<MortalityDTO>> GetByDate(DateTime dateDeath)
    {
        return await _context.Mortalities
        .Where(m => m.DateDeath.Date == dateDeath.Date)
        .Select(e => new MortalityDTO
        {
            Id = e.Id,
            DateDeath = e.DateDeath,
            DeathQuantity = e.DeathQuantity,
            CutQuantity = e.CutQuantity,
            Reason = e.Reason,
            LotId = e.LotId
        }).ToListAsync();
    }
    public async Task<MortalityDTO> Create(MortalityDTO mortalityDto)
    {
        var mortality = new Mortality
        {
            DateDeath = mortalityDto.DateDeath,
            DeathQuantity = mortalityDto.DeathQuantity,
            CutQuantity = mortalityDto.CutQuantity,
            Reason = mortalityDto.Reason,
            LotId = mortalityDto.LotId
        };
        _context.Mortalities.Add(mortality);
        await _context.SaveChangesAsync();
        mortalityDto.Id = mortality.Id;
        return mortalityDto;
    }
}
