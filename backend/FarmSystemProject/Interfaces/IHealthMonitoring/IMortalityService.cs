using FarmSystemProject.DTOs.HealthMonitoringDTO;

namespace FarmSystemProject.Interfaces.IHealthMonitoring;

public interface IMortalityService
{
    Task<MortalityResponse> Create(int lotId, int ownerId, CreateMortalityRequest request);
    Task<MortalityDateSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date);
    Task<IEnumerable<MortalityResponse>> GetAllByLotId(int lotId, int ownerId);
}