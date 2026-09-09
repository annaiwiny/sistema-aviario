using FarmSystemProject.Data;
using FarmSystemProject.DTOs.ProductiveMonitoringDTO;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.INotifications;
using FarmSystemProject.Interfaces.IProductiveMonitoring;
using FarmSystemProject.Models.Lots;
using FarmSystemProject.Models.ProductiveMonitoring;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.ProductiveMonitoringService;

public class EggProductionService : IEggProductionService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public EggProductionService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<EggProductionResponse> Create(int lotId, int ownerId, CreateEggProductionRequest request)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado.");

        var birdsAliveOnDate = await GetBirdsAliveOnDate(lot, request.ProductionDate);

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

        // Gera alerta apenas se houver queda brusca em relação à média recente
        await _notificationService.CheckEggProduction(lotId, production.ProductionDate);

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

    public async Task<EggProductionResponse> UpdateByDate(int lotId, int ownerId, UpdateEggProductionRequest request)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado.");

        var records = await _context.EggProductions
            .Where(e => e.LotId == lotId && e.ProductionDate.Date == request.ProductionDate.Date)
            .OrderBy(e => e.Id)
            .ToListAsync();

        if (records.Count == 0)
            throw new NotFoundException("Não há coleta registrada nessa data para corrigir.");

        var birdsAliveOnDate = await GetBirdsAliveOnDate(lot, request.ProductionDate);

        if (request.Quantity > birdsAliveOnDate)
            throw new BusinessException($"Erro: Você informou {request.Quantity} ovos, mas o lote só possui {birdsAliveOnDate}.");

        // O dia fica com um único lançamento valendo o total corrigido. Se os
        // demais lançamentos do mesmo dia continuassem existindo, o relatório e o
        // gráfico voltariam a somá-los em cima da correção.
        var production = records[0];
        production.Quantity = request.Quantity;

        if (records.Count > 1)
            _context.EggProductions.RemoveRange(records.Skip(1));

        await _context.SaveChangesAsync();

        await _notificationService.CheckEggProduction(lotId, production.ProductionDate);

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

    // Aves vivas no início do dia informado: estoque inicial do lote menos as
    // mortes e descartes lançados em dias ANTERIORES.
    //
    // O '<' (e não '<=') importa: uma ave que morreu no dia 8 estava viva na
    // manhã do dia 8 e pôs ovo. Descontando-a, o cadastro recusava uma coleta
    // legítima ("você informou 380 ovos, mas o lote só possui 350") e o número
    // divergia do painel do lote, que sempre usou o dia anterior.
    private async Task<int> GetBirdsAliveOnDate(Lot lot, DateTime date)
    {
        var initialStock = lot.Lineages.Sum(x => x.Quantity);

        var previousLosses = await _context.Mortalities
            .Where(m => m.LotId == lot.Id && m.DateDeath.Date < date.Date)
            .SumAsync(m => m.DeathQuantity + m.CutQuantity);

        return initialStock - previousLosses;
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