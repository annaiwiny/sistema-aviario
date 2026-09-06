using FarmSystemProject.Data;
using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Models.HealthMonitoring;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.HealthMonitoringService;

public class VaccinationService : IVaccinationService
{
    private readonly AppDbContext _context;

    public VaccinationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VaccinationResponse> Create(int lotId, int ownerId, CreateVaccinationRequest request)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var vaccination = new Vaccination
        {
            LotId = lotId,
            ApplicationDate = request.ApplicationDate,
            VaccineType = request.VaccineType,
            ApplicationValue = request.ApplicationValue,
            ApplicationQuantity = request.ApplicationQuantity
        };

        _context.Vaccinations.Add(vaccination);
        await _context.SaveChangesAsync();

        return new VaccinationResponse
        {
            Id = vaccination.Id,
            ApplicationDate = vaccination.ApplicationDate,
            VaccineType = vaccination.VaccineType,
            ApplicationValue = vaccination.ApplicationValue,
            ApplicationQuantity = vaccination.ApplicationQuantity,
            TotalCost = vaccination.ApplicationValue * vaccination.ApplicationQuantity,
            LotId = vaccination.LotId
        };
    }

    public async Task<VaccinationDateSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date)
    {
        var dailyRecords = await _context.Vaccinations
            .Where(v => v.LotId == lotId && v.ApplicationDate.Date == date.Date)
            .ToListAsync();

        if (dailyRecords.Count == 0)
            throw new NotFoundException("Não há dados referente a data informada");

        if(!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var totalQuantity = dailyRecords.Sum(r => r.ApplicationQuantity);
        var totalCost = dailyRecords.Sum(r => r.ApplicationValue * r.ApplicationQuantity);

        var summary = new VaccinationDateSummary
        {
            Date = date,
            TotalQuantity = totalQuantity,
            TotalCost = totalCost,
            VaccineTypes = string.Join(", ", dailyRecords.Select(r => r.VaccineType).Distinct()) // Concatena os tipos distintos
        };

        // Se houver só 1 registro, será exato. Se houver vários, será a média.
        if (totalQuantity > 0)
            summary.ApplicationValue = Math.Round(totalCost / totalQuantity, 2);

        return summary;
    }

    public async Task<IEnumerable<VaccinationResponse>> GetAllByLotId(int lotId, int ownerId)
    {
        if (!await _context.Lots.AnyAsync(l => l.Id == lotId && l.Farm.OwnerId == ownerId))
            throw new NotFoundException("Lote não encontrado.");

        var vaccinations = await _context.Vaccinations
                    .AsNoTracking()
                    .Where(v => v.LotId == lotId)
                    .OrderByDescending(v => v.ApplicationDate)
                    .Select(v => new VaccinationResponse
                    {
                        Id = v.Id,
                        ApplicationDate = v.ApplicationDate,
                        VaccineType = v.VaccineType,
                        ApplicationValue = v.ApplicationValue,
                        ApplicationQuantity = v.ApplicationQuantity,
                        TotalCost = v.ApplicationValue * v.ApplicationQuantity,
                        LotId = v.LotId
                    })
                    .ToListAsync();

        return vaccinations;
    }
}