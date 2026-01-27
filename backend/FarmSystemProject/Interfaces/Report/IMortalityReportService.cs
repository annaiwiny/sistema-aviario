namespace FarmSystemProject.Interfaces;

public interface IMortalityReportService
{
    Task<byte[]> GenerateMortalityListReport();
    Task<byte[]> GenerateMortalityDateReport(DateTime dateDeath);
}