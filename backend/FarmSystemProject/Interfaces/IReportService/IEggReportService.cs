namespace FarmSystemProject.Interfaces.IReportService;

public interface IEggReportService
{
    Task<byte[]> GenerateEggListReport();
    Task<byte[]> GenerateEggDateReport(DateTime collectDate);
}
