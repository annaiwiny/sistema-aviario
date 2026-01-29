using FarmSystemProject.Data;
using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Models.ProductiveMonitoring;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.ProductiveMonitoringService;

public class EggProductionService : IEggProductionService
{
    private readonly AppDbContext _context;

    public EggProductionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EggProductionResponse> Create(int lotId, int ownerId, CreateEggProductionRequest request)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado.");

        // Quantidade inicial de galinhas (Lá no cadastro do novo lote)
        var initialStock = lot.Lineages.Sum(x => x.Quantity);

        // Quantidade de galinhas que morreram anteriores a data informada
        var previousLosses = await _context.Mortalities
            .Where(m => m.LotId == lotId && m.DateDeath.Date <= request.ProductionDate.Date)
            .SumAsync(m => m.DeathQuantity + m.CutQuantity);

        // Total de aves vivas no início do dia consultado
        var birdsAliveOnDate = initialStock - previousLosses;

        if (request.Quantity > birdsAliveOnDate)
            throw new BusinessException($"Erro: Você informou {request.Quantity} ovos, mas o lote só possui {birdsAliveOnDate}.");

        var production = new EggProduction
        {
            LotId = lotId,
            ProductionDate = request.ProductionDate,
            Quantity = request.Quantity
        };

        _context.EggProductions.Add(production);
        await _context.SaveChangesAsync();

        // Proteção contra divisão por zero (caso todas tenham morrido antes)
        if (birdsAliveOnDate <= 0)
            birdsAliveOnDate = 1;

        var rate = birdsAliveOnDate > 0
            ? Math.Round(((decimal)production.Quantity / birdsAliveOnDate) * 100, 2)
            : 0;

        return new EggProductionResponse
        {
            Id = production.Id,
            ProductionDate = production.ProductionDate,
            Quantity = production.Quantity,
            LotId = production.LotId,
            LayingRate = rate
        };
    }

    public async Task<EggProductionDateSummary?> GetSummaryByDate(int lotId, int ownerId, DateTime date)
    {
        if(!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var dailyRecords = await _context.EggProductions
            .AsNoTracking()
            .Where(e => e.LotId == lotId && e.ProductionDate.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0)
            throw new NotFoundException("Não há dados referente a data informada");

        var totalQuantity = dailyRecords.Sum(r => r.Quantity);

        return new EggProductionDateSummary
        {
            Date = date,
            TotalQuantity = totalQuantity
        };
    }

    public async Task<IEnumerable<EggProductionResponse>> GetAllByLotId(int lotId, int ownerId)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var productions = await _context.EggProductions
            .AsNoTracking()
            .Where(e => e.LotId == lotId)
            .OrderByDescending(e => e.ProductionDate)
            .Select(p => new EggProductionResponse
            {
                Id = p.Id,
                ProductionDate = p.ProductionDate,
                Quantity = p.Quantity,
                LotId = p.LotId,
                LayingRate = 0
            })
            .ToListAsync();

        return productions;
    }
}