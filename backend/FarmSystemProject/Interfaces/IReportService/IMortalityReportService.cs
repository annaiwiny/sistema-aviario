namespace FarmSystemProject.Interfaces.IReportService;

public interface IMortalityReportService
{
    Task<byte[]> GenerateMortalityListReport(int lotId, int ownerId);
    Task<byte[]> GenerateMortalityDateReport(int lotId, int ownerId, DateTime dateDeath);
}