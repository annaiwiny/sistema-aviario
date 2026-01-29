namespace FarmSystemProject.Interfaces.IReportInterface;

public interface IEggProductionReportService
{
    Task<byte[]> GenerateEggListReport(int lotId, int ownerId);
    Task<byte[]> GenerateEggDateReport(int lotId, int ownerId, DateTime date);
}