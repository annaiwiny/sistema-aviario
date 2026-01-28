using FarmSystemProject.Data;
using FarmSystemProject.DTOs.FarmDTO;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Models.Farm;
using FarmSystemProject.Services.Interfaces.IFarm;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.FarmService;

public class FarmService : IFarmService
{
    private readonly AppDbContext _context;

    public FarmService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FarmResponse> CreateAsync(int ownerId, CreateFarmRequest request)
    {
        if(await _context.Farms.AnyAsync(f => f.OwnerId == ownerId))
        {
            throw new BusinessException("Você já possui um aviário cadastrado");
        };

        var farm = new Farm
        {
            Name = request.Name,
            OwnerId = ownerId,
        };

        _context.Farms.Add(farm);
        await _context.SaveChangesAsync();

        return new FarmResponse
        {
            Id = farm.Id,
            Name = farm.Name,
            OwnerId = farm.OwnerId,
            Lots = []
        };
    }

    public async Task<FarmResponse?> GetByOwnerIdAsync(int ownerId)
    {
        var farm = await _context.Farms
            .AsNoTracking()
            .Include(f => f.Lots)
            .FirstOrDefaultAsync(f => f.OwnerId == ownerId);

        if (farm == null)
            throw new NotFoundException("Aviário não encontrado");

        return new FarmResponse
        {
            Id = farm.Id,
            Name = farm.Name,
            OwnerId = farm.OwnerId,
            Lots = farm.Lots
                .OrderByDescending(l => l.AccommodationDate) // Serve para ordenar lotes pela data.
                .Select(l => new LotSummaryResponse
                {
                    Id = l.Id,
                    AccommodationDate = l.AccommodationDate
                }).ToList()
        };
    }
}