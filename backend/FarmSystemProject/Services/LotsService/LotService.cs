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
        var farm = await _context.Farms
            .Where(f => f.OwnerId == ownerId)
            .Select(f => new { f.Id })
            .FirstOrDefaultAsync();

        if (farm == null)
            throw new NotFoundException("Você não possui nenhum aviário cadastrado");

        var lot = new Lot
        {
            FarmId = farm.Id,
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
}