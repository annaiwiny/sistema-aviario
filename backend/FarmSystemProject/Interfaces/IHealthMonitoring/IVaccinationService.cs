using FarmSystemProject.DTOs.HealthMonitoringDTO;

namespace FarmSystemProject.Interfaces.IHealthMonitoring;
public interface IVaccinationService
{
    Task<IEnumerable<VaccinationDTO>> GetAll();
    Task<IEnumerable<VaccinationDTO>> GetByDate(DateTime applicationDate);
    Task<VaccinationDTO> Create(VaccinationDTO vaccinationDto);
}
