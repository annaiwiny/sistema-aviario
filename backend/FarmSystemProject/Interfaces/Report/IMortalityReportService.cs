namespace FarmSystemProject.Interfaces;

public interface IMortalityReportService
{
    Task<byte[]> GenerateMortalityListReport();
}