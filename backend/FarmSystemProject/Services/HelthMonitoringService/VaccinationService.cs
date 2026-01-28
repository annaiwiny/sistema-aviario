using FarmSystemProject.Data;
using FarmSystemProject.DTOs.HealthMonitoringDTO;
using FarmSystemProject.Interfaces.IHealthMonitoring;
using FarmSystemProject.Models.HealthMonitoring;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Services.HelthMonitoringService;
public class VaccinationService : IVaccinationService
{
    private readonly AppDbContext _context;
    public VaccinationService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<VaccinationDTO>> GetAll()
    {
        return await _context.Vaccinations.Select(m => new VaccinationDTO
        {
            Id = m.Id,
            ApplicationDate = m.ApplicationDate,
            VaccineType = m.VaccineType,
            ApplicationValue = m.ApplicationValue,
            ApplicationQuantity = m.ApplicationQuantity,
            LotId = m.LotId
        }).ToListAsync();
    }
    public async Task<IEnumerable<VaccinationDTO>> GetByDate(DateTime applicationDate)
    {
        return await _context.Vaccinations
        .Where(d => d.ApplicationDate.Date == applicationDate.Date)
        .Select(m => new VaccinationDTO
        {
            Id = m.Id,
            ApplicationDate = m.ApplicationDate,
            VaccineType = m.VaccineType,
            ApplicationValue = m.ApplicationValue,
            ApplicationQuantity = m.ApplicationQuantity,
            LotId = m.LotId
        }).ToListAsync();
    }
    public async Task<VaccinationDTO> Create(VaccinationDTO vaccinationDto)
    {
        var vaccination = new Vaccination
        {
            ApplicationDate = vaccinationDto.ApplicationDate,
            VaccineType = vaccinationDto.VaccineType,
            ApplicationValue = vaccinationDto.ApplicationValue,
            ApplicationQuantity = vaccinationDto.ApplicationQuantity,
            LotId = vaccinationDto.LotId
        };
        _context.Vaccinations.Add(vaccination);
        await _context.SaveChangesAsync();
        vaccinationDto.Id = vaccination.Id;
        return vaccinationDto;
    }
}
