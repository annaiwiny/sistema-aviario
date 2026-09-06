using FarmSystemProject.DTOs.ProductiveMonitoringDTO;

namespace FarmSystemProject.Interfaces.IProductiveMonitoring;

public interface IEggProductionService
{
    Task<EggProductionResponse> Create(int lotId, int ownerId, CreateEggProductionRequest request);
    Task<IEnumerable<EggProductionResponse>> GetAllByLotId(int lotId, int ownerId);
    Task<EggProductionDateSummary?> GetSummaryByDate(int lotId, int ownerId, DateTime date);
}