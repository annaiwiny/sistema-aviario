using FarmSystemProject.Data;
using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Models.ProductiveMonitoring;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.ProductiveMonitoringService;
public class EggService : IEggService
{
    private readonly AppDbContext _context;
    public EggService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<EggDTO>> GetAll()
    {
        return await _context.CollectEggs.Select(e => new EggDTO
        {
            Id = e.Id,
            CollectDate = e.CollectDate,
            CollectQuantity = e.CollectQuantity,
            LotId = e.LotId
        }).ToListAsync();
    }
    public async Task<IEnumerable<EggDTO>> GetByDate(DateTime collectDate)
    {
        return await _context.CollectEggs
        .Where(e => e.CollectDate.Date == collectDate.Date)
        .Select(e => new EggDTO
        {
            Id = e.Id,
            CollectDate = e.CollectDate,
            CollectQuantity = e.CollectQuantity,
            LotId = e.LotId
        }).ToListAsync();
    }
    public async Task<EggDTO> Create(EggDTO eggDto)
    {
        var egg = new CollectEgg
        {
            CollectDate = eggDto.CollectDate,
            CollectQuantity = eggDto.CollectQuantity,
            LotId = eggDto.LotId
        };
        _context.CollectEggs.Add(egg);
        await _context.SaveChangesAsync();
        eggDto.Id = egg.Id;
        return eggDto;
    }
}
