using FarmSystemProject.Data;
using FarmSystemProject.DTOs.FarmDTO;
using FarmSystemProject.Interfaces.IFarm;
using FarmSystemProject.Models.Farm;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.FarmService;
public class LotService : ILotService
{
    private readonly AppDbContext _context;
    public LotService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<LotDTO>> GetAll()
    {
        return await _context.Lots
            .Include(l => l.Items).Select(l => new LotDTO
            {
                Id = l.Id,
                AccommodationDate = l.AccommodationDate,
                FarmId = l.FarmId,
                Items = l.Items.Select(i => new LotItemDTO
                {
                    Race = i.Race,
                    Quantity = i.Quantity
                }).ToList()
            }).ToListAsync();
    }
    public async Task<LotDTO> Create(LotDTO lotDto)
    {
        var lot = new Lot
        {
            AccommodationDate = lotDto.AccommodationDate,
            FarmId = lotDto.FarmId,
            Items = lotDto.Items.Select(i => new LotItem
            {
                Race = i.Race,
                Quantity = i.Quantity
            }).ToList()
        };

        _context.Lots.Add(lot);
        await _context.SaveChangesAsync();
        lotDto.Id = lot.Id;
        return lotDto;
    }

    public async Task<LotDTO?> GetById(int id)
    {
        var lot = await _context.Lots.Include(l => l.Items).FirstOrDefaultAsync(l => l.Id == id);
        if (lot == null)
        {
            return null;
        }

        return new LotDTO
        {
            Id = lot.Id,
            AccommodationDate = lot.AccommodationDate,
            FarmId = lot.FarmId,
            Items = lot.Items.Select(i => new LotItemDTO
            {
                Race = i.Race,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
