using FarmSystemProject.DTOs.HealthMonitoringDTO;

namespace FarmSystemProject.Interfaces.IHealthMonitoring;

public interface IVaccinationService
{
    Task<VaccinationResponse> Create(int lotId, int ownerId, CreateVaccinationRequest request);
    Task<IEnumerable<VaccinationResponse>> GetAllByLotId(int lotId, int ownerId);
    Task<VaccinationDateSummary> GetSummaryByDate(int lotId, int ownerId, DateTime date);
}