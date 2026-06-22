using FarmSystemProject.Data;
using FarmSystemProject.DTOs.Lots;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.ILots;
using FarmSystemProject.Models.Lots;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.LotsService;
public class LotService : ILotService
{
    private readonly AppDbContext _context;

    public LotService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LotResponse> Create(int ownerId, CreateLotRequest request)
    {
        var farmId = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        if (farmId == 0)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        var lot = new Lot
        {
            FarmId = farmId,
            AccommodationDate = request.AccommodationDate,
            Lineages = request.Lineages.Select(l => new Lineage
            {
                Race = l.Race,
                Quantity = l.Quantity
            }).ToList()
        };

        _context.Lots.Add(lot);
        await _context.SaveChangesAsync();

        return new LotResponse
        {
            Id = lot.Id,
            AccommodationDate = lot.AccommodationDate,
            FarmId = lot.FarmId,
            Lineages = lot.Lineages.Select(i => new LineageResponse
            {
                Id = i.Id,
                Race = i.Race,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<LotResponse> GetById(int id, int ownerId)
    {
        var farmId = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        // Se farmId for 0 (default do int), significa que não achou nada
        if (farmId == 0)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == id && l.FarmId == farmId); // Garante que o Lote realmente pertence ao usuário.

        if (lot == null)
            throw new NotFoundException("Lote não encontrado");

        return new LotResponse
        {
            Id = lot.Id,
            AccommodationDate = lot.AccommodationDate,
            FarmId = lot.FarmId,
            Lineages = lot.Lineages.Select(i => new LineageResponse
            {
                Id = i.Id,
                Race = i.Race,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<LotResponse> UpdateAsync(int id, int ownerId, UpdateLotRequest request)
    {
        var farmId = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => f.Id)
            .FirstOrDefaultAsync();

        if (farmId == 0)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        var lot = await _context.Lots
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == id && l.FarmId == farmId);

        if (lot == null)
            throw new NotFoundException("Lote não encontrado");

        if (request.AccommodationDate.HasValue)
            lot.AccommodationDate = request.AccommodationDate.Value;

        if (request.Lineages != null)
        {
            if (request.Lineages.Count == 0)
                throw new BusinessException("O lote não pode ficar sem nenhuma linhagem.");

            // Remove os lotes antigos
            _context.RemoveRange(lot.Lineages);

            // Adiciona os lotes novos
            lot.Lineages = request.Lineages.Select(l => new Lineage
            {
                Race = l.Race,
                Quantity = l.Quantity,
                LotId = lot.Id
            }).ToList();
        }

        await _context.SaveChangesAsync();

        return new LotResponse
        {
            Id = lot.Id,
            AccommodationDate = lot.AccommodationDate,
            FarmId = lot.FarmId,
            Lineages = lot.Lineages.Select(i => new LineageResponse
            {
                Id = i.Id,
                Race = i.Race,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<LotDashboardResponse> GetDashboardSummary(int lotId, int ownerId)
    {
        var lot = await _context.Lots
            .AsNoTracking()
            .Include(l => l.Lineages)
            .FirstOrDefaultAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId);

        if (lot == null) 
            throw new NotFoundException("Lote não encontrado.");

        var today = DateTime.Today;
        // Quantidade inicial de galinhas
        var initialStock = lot.Lineages.Sum(x => x.Quantity);

        // Mortes anteriores a data de hoje
        var previousLosses = await _context.Mortalities
            .Where(m => m.LotId == lotId && m.DateDeath.Date < today) 
            .SumAsync(m => m.DeathQuantity + m.CutQuantity);

        // Quantidade de galinhas vivas hoje
        var birdsStartOfDay = initialStock - previousLosses;

        // Ovos coletados hoje
        var eggsToday = await _context.EggProductions
            .Where(e => e.LotId == lotId && e.ProductionDate.Date == today)
            .SumAsync(e => e.Quantity);

        var notLaying = birdsStartOfDay - eggsToday;
        // Evita valores negativos que quebrariam o frontend ao retornar.
        if (notLaying < 0) 
            notLaying = 0;
        
        decimal percentage = 0;
        if (birdsStartOfDay > 0)
        {
            percentage = ((decimal)eggsToday / birdsStartOfDay) * 100;
        }

        return new LotDashboardResponse
        {
            LotId = lot.Id,
            CurrentAlive = birdsStartOfDay,    
            EggsCollectedToday = eggsToday,    
            HensNotLayingToday = notLaying,    
            LayingPercentage = Math.Round(percentage, 2)
        };
    }
}