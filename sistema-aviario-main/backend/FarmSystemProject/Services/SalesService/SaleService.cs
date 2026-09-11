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

    // A granja do usuário logado. Cada dono tem no máximo uma (índice único em
    // Farm.OwnerId), então é ela que amarra todas as vendas.
    private async Task<int> GetFarmId(int ownerId)
    {
        var farmId = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        if (farmId == 0)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        return farmId;
    }

    public async Task<SaleRecordResponse> Create(int ownerId, CreateSaleRecord request)
    {
        var farmId = await GetFarmId(ownerId);

        var sale = new Sale
        {
            UnitValue = request.UnitValue,
            EggQuantity = request.EggQuantity,
            SaleDate = request.SaleDate,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            FarmId = farmId,
            LotId = null
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
            Notes = sale.Notes,
            FarmId = sale.FarmId,
            LotId = sale.LotId
        };
    }

    public async Task<SaleRecordSummary> GetSummaryByDate(int ownerId, DateTime date)
    {
        var farmId = await GetFarmId(ownerId);

        var dailyRecords = await _context.Sales
            .AsNoTracking()
            .Where(s => s.FarmId == farmId && s.SaleDate.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0)
            throw new NotFoundException("Não há dados referente a data informada");

        var totalEggQuantity = dailyRecords.Sum(s => s.EggQuantity);
        var totalValue = dailyRecords.Sum(s => s.TotalValue);

        var dailyNotes = dailyRecords
            .Where(s => !string.IsNullOrWhiteSpace(s.Notes))
            .Select(s => s.Notes!.Trim())
            .ToList();

        return new SaleRecordSummary
        {
            SaleDate = date,
            // Média ponderada do dia. Divisão protegida: uma venda lançada com
            // zero ovos derrubaria a tela de relatório com erro 500.
            UnitValue = totalEggQuantity > 0 ? Math.Round(totalValue / totalEggQuantity, 2) : 0,
            EggQuantity = totalEggQuantity,
            TotalValue = totalValue,
            Notes = dailyNotes.Count > 0 ? string.Join(" | ", dailyNotes) : null,
        };
    }

    public async Task<IEnumerable<SaleRecordResponse>> GetAllByFarm(int ownerId)
    {
        var farmId = await GetFarmId(ownerId);

        return await _context.Sales
            .AsNoTracking()
            .Where(s => s.FarmId == farmId)
            .OrderByDescending(s => s.SaleDate)
            .Select(s => new SaleRecordResponse
            {
                Id = s.Id,
                UnitValue = s.UnitValue,
                EggQuantity = s.EggQuantity,
                TotalValue = s.UnitValue * s.EggQuantity,
                SaleDate = s.SaleDate,
                Notes = s.Notes,
                FarmId = s.FarmId,
                LotId = s.LotId
            })
            .ToListAsync();
    }
}
