using FarmSystemProject.Data;
using FarmSystemProject.DTOs.NutritionalControl.Feeding;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.INutritionalControl;
using FarmSystemProject.Models.NutritionalControl;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.NutritionalControl;

public class FeedingService : IFeedingService
{
    private readonly AppDbContext _context;

    public FeedingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FeedingResponse> Create(int lotId, int ownerId, CreateFeeding request)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var feeding = new Feeding
        {
            ConsumptionQuantity = request.ConsumptionQuantity,
            ConsumptionDate = request.ConsumptionDate,
            LotId = lotId
        };

        _context.Feedings.Add(feeding);
        await _context.SaveChangesAsync();

        return new FeedingResponse
        {
            Id = feeding.Id,
            ConsumptionQuantity = feeding.ConsumptionQuantity,
            ConsumptionDate = feeding.ConsumptionDate,
            LotId = feeding.LotId
        };
    }

    public async Task<FeedingSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var dailyRecords = await _context.Feedings
            .Where(f => f.LotId == lotId && f.ConsumptionDate.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0)
            throw new NotFoundException("Não há dados referente a data informada");

        var summary = new FeedingSummary
        {
            ConsumptionDate = date,
            ConsumptionQuantity = dailyRecords.Sum(f => f.ConsumptionQuantity)
        };

        return summary;
    }

    public async Task<IEnumerable<FeedingResponse>> GetAllByLotId(int lotId, int ownerId)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var feedings = await _context.Feedings
                    .AsNoTracking()
                    .Where(f => f.LotId == lotId)
                    .OrderByDescending(f => f.ConsumptionDate)
                    .Select(f => new FeedingResponse
                    {
                        Id = f.Id,
                        ConsumptionQuantity = f.ConsumptionQuantity,
                        ConsumptionDate = f.ConsumptionDate,
                        LotId = f.LotId
                    })
                    .ToListAsync();

        return feedings;
    }
}