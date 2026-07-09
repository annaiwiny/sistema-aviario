using FarmSystemProject.Data;
using FarmSystemProject.DTOs.NutritionalControl.Feed;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.INutritionalControl;
using FarmSystemProject.Models.NutritionalControl;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.NutritionalControl;

public class FeedService : IFeedService
{
    private readonly AppDbContext _context;

    public FeedService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FeedResponse> Create(int lotId, int ownerId, CreateFeed request)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var feed = new Feed
        {
            PurchaseDate = request.PurchaseDate,
            BagWeight = request.BagWeight,
            BagQuantity = request.BagQuantity,
            BagValue = request.BagValue,
            LotId = lotId
        };

        _context.Feeds.Add(feed);
        await _context.SaveChangesAsync();

        return new FeedResponse
        {
            Id = feed.Id,
            PurchaseDate = feed.PurchaseDate,
            BagWeight = feed.BagWeight,
            BagQuantity = feed.BagQuantity,
            BagValue = feed.BagValue,
            TotalCost = feed.TotalCost,
            LotId = feed.LotId
        };
    }

    public async Task<FeedSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var dailyRecords = await _context.Feeds
            .Where(f => f.LotId == lotId && f.PurchaseDate.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0)
            throw new NotFoundException("Não há dados referente à data informada.");

        var totalBagQuantity = dailyRecords.Sum(f => f.BagQuantity);

        var summary = new FeedSummary
        {
            PurchaseDate = date,
            BagWeight = Math.Round(dailyRecords.Sum(f => f.BagWeight * f.BagQuantity) / totalBagQuantity, 2),
            BagValue = Math.Round(dailyRecords.Sum(f => f.BagValue * f.BagQuantity) / totalBagQuantity, 2),
            BagQuantity = totalBagQuantity,
            TotalCost = dailyRecords.Sum(f => f.TotalCost)
        };

        return summary;
    }

    public async Task<IEnumerable<FeedResponse>> GetAllByLotId(int lotId, int ownerId)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var feeds = await _context.Feeds
            .AsNoTracking()
            .Where(f => f.LotId == lotId)
            .OrderByDescending(f => f.PurchaseDate)
            .Select(f => new FeedResponse
            {
                Id = f.Id,
                PurchaseDate = f.PurchaseDate,
                BagWeight = f.BagWeight,
                BagQuantity = f.BagQuantity,
                BagValue = f.BagValue,
                TotalCost = f.TotalCost,
                LotId = f.LotId
            })
            .ToListAsync();

        return feeds;
    }
}
