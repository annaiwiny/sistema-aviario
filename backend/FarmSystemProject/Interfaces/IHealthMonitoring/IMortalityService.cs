using FarmSystemProject.DTOs.HealthMonitoringDTO;

namespace FarmSystemProject.Interfaces.IHealthMonitoring;
public interface IMortalityService
{
    Task<IEnumerable<MortalityDTO>> GetAll();
    Task<IEnumerable<MortalityDTO>> GetByDate(DateTime dateDeath);
    Task<MortalityDTO> Create(MortalityDTO mortalityDto);
}
