namespace FarmSystemProject.Interfaces;

public interface IVaccinationReportService
{
    Task<byte[]> GenerateVaccinationListReport();
    Task<byte[]> GenerateVaccinationDateReport(DateTime applicationDate);
}
