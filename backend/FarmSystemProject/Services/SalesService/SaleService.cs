using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Sales;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.ISales;
using FarmSystemProject.Models.Sales;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.Sales;

public class SaleService : ISaleService
{
    private readonly AppDbContext _context;

    public SaleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SaleRecordResponse> Create(int lotId, int ownerId, CreateSaleRecord request)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var sale = new Sale
        {
            UnitValue = request.UnitValue,
            EggQuantity = request.EggQuantity,
            SaleDate = request.SaleDate,
            LotId = lotId
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return new SaleRecordResponse
        {
            Id = sale.Id,
            UnitValue = sale.UnitValue,
            EggQuantity = sale.EggQuantity,
            TotalValue = sale.TotalValue,
            SaleDate = sale.SaleDate,
            LotId = sale.LotId
        };
    }

    public async Task<SaleRecordSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var dailyRecords = await _context.Sales
            .Where(s => s.LotId == lotId && s.SaleDate.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0)
            throw new NotFoundException("Não há dados referente a data informada");

        var totalEggQuantity = dailyRecords.Sum(s => s.EggQuantity);
        var totalValue = dailyRecords.Sum(s => s.TotalValue);

        var summary = new SaleRecordSummary
        {
            SaleDate = date,
            UnitValue = Math.Round(totalValue / totalEggQuantity, 2),
            EggQuantity = totalEggQuantity,
            TotalValue = totalValue,
        };

        return summary;
    }

    public async Task<IEnumerable<SaleRecordResponse>> GetAllByLotId(int lotId, int ownerId)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var sales = await _context.Sales
                    .AsNoTracking()
                    .Where(s => s.LotId == lotId)
                    .OrderByDescending(s => s.SaleDate)
                    .Select(s => new SaleRecordResponse
                    {
                        Id = s.Id,
                        UnitValue = s.UnitValue,
                        EggQuantity = s.EggQuantity,
                        TotalValue = s.TotalValue,
                        SaleDate = s.SaleDate,
                        LotId = s.LotId
                    })
                    .ToListAsync();

        return sales;
    }
}