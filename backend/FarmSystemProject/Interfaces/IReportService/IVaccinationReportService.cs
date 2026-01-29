namespace FarmSystemProject.Interfaces.IReportService;

public interface IVaccinationReportService
{
    Task<byte[]> GenerateVaccinationListReport(int lotId, int ownerId);
    Task<byte[]> GenerateVaccinationDateReport(int lotId, int ownerId, DateTime date);
}