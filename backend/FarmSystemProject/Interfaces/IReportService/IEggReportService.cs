namespace FarmSystemProject.Interfaces;

public interface IEggReportService
{
    Task<byte[]> GenerateEggListReport();
    Task<byte[]> GenerateEggDateReport(DateTime collectDate);
}
