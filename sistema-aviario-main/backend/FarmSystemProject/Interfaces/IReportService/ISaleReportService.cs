namespace FarmSystemProject.Interfaces.IReportService;

public interface ISaleReportService
{
    Task<byte[]> GenerateSalesListReport(int lotId, int ownerId);
    Task<byte[]> GenerateSalesDateReport(int lotId, int ownerId, DateTime date);
}