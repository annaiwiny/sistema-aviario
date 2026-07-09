namespace FarmSystemProject.Interfaces.IReportService;

public interface IFeedingReportService
{
    Task<byte[]> GenerateFeedingListReport(int lotId, int ownerId);
    Task<byte[]> GenerateFeedingDateReport(int lotId, int ownerId, DateTime date);
}
