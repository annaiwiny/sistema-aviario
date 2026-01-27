using FarmSystemProject.DTOs.ProductiveMonitoringDTO;

namespace FarmSystemProject.Interfaces.IProductiveMonitoring;
public interface IEggService
{
    Task<IEnumerable<EggDTO>> GetAll();
    Task<IEnumerable<EggDTO>> GetByDate(DateTime collectDate);
    Task<EggDTO> Create(EggDTO eggDto);
}
