using FarmSystemProject.Data;
using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.Exceptions;
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

    public async Task<MortalityResponse> Create(int lotId, int ownerId, CreateMortalityRequest request)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado.");

        // Quantidade inicial de galinhas (Lá no cadastro do novo lote)
        var initialStock = lot.Lineages.Sum(x => x.Quantity);

        // Quantidade de galinhas que já morreram
        var previousLosses = await _context.Mortalities
            .Where(m => m.LotId == lotId)
            .SumAsync(m => m.DeathQuantity + m.CutQuantity);

        // Quantidade de galinhas vivas
        var currentStock = initialStock - previousLosses;

        if (request.DeathQuantity + request.CutQuantity > currentStock)
            throw new BusinessException($"A quantidade informada excede o total de aves vivas no lote ({currentStock}).");

        var mortality = new Mortality
        {
            LotId = lotId,
            DateDeath = request.DateDeath,
            DeathQuantity = request.DeathQuantity,
            CutQuantity = request.CutQuantity,
            Reason = request.Reason
        };

        _context.Mortalities.Add(mortality);
        await _context.SaveChangesAsync();

        return new MortalityResponse
        {
            Id = mortality.Id,
            DateDeath = mortality.DateDeath,
            DeathQuantity = mortality.DeathQuantity,
            CutQuantity = mortality.CutQuantity,
            Reason = mortality.Reason,
            LotId = mortality.LotId
        };
    }

    public async Task<MortalityDateSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date)
    {
        var dailyRecords = await _context.Mortalities
            .Where(m => m.LotId == lotId && m.DateDeath.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0) 
            throw new NotFoundException("Não há dados referente a data informada");
    
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId);

        if (lot == null) 
            throw new NotFoundException("Lote não encontrado.");

        var initialStock = lot.Lineages.Sum(x => x.Quantity);

        // Quantidade de galinhas que morreram anteriores a data informada
        var previousLosses = await _context.Mortalities
             .Where(m => m.LotId == lotId && m.DateDeath.Date < date.Date)
             .SumAsync(m => m.DeathQuantity + m.CutQuantity);

        // Total de aves vivas no início do dia consultado
        var birdsAliveOnDate = initialStock - previousLosses;

        // Proteção contra divisão por zero (caso todas tenham morrido antes)
        if (birdsAliveOnDate <= 0)
            birdsAliveOnDate = 1;

        var summary = new MortalityDateSummary
        {
            Date = date,
            TotalDeaths = dailyRecords.Sum(r => r.DeathQuantity),
            TotalCuts = dailyRecords.Sum(r => r.CutQuantity),
            Motives = string.Join(", ", dailyRecords.Select(r => r.Reason).Distinct()) // Concatena os motivos distintos
        };

        // Fórmula: (Galinhas mortas no dia / Total de aves do lote) * 100
        decimal rate = ((decimal)summary.TotalDeaths / birdsAliveOnDate) * 100;
        summary.MortalityRate = Math.Round(rate, 2);

        return summary;
    }

    public async Task<IEnumerable<MortalityResponse>> GetAllByLotId(int lotId, int ownerId)
    {
        if(!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var mortalities = await _context.Mortalities
            .AsNoTracking()
            .Where(m => m.LotId == lotId)
            .OrderByDescending(m => m.DateDeath)
            .Select(m => new MortalityResponse
            {
                Id = m.Id,
                DateDeath = m.DateDeath,
                DeathQuantity = m.DeathQuantity,
                CutQuantity = m.CutQuantity,
                Reason = m.Reason,
                LotId = m.LotId
            })
            .ToListAsync();

        return mortalities;
    }
}