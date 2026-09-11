namespace FarmSystemProject.Interfaces.IReportService;

public interface ISaleReportService
{
    Task<byte[]> GenerateSalesListReport(int ownerId);
    Task<byte[]> GenerateSalesDateReport(int ownerId, DateTime date);
}
