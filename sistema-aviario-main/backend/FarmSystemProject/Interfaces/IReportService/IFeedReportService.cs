namespace FarmSystemProject.Interfaces.IReportService;

public interface IFeedReportService
{
    Task<byte[]> GenerateFeedListReport(int lotId, int ownerId);
    Task<byte[]> GenerateFeedDateReport(int lotId, int ownerId, DateTime date);
}
