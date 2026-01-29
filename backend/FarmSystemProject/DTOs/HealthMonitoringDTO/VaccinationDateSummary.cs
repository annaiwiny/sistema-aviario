namespace FarmSystemProject.DTOs.HealthMonitoringDTO;

public class VaccinationDateSummary
{
    public DateTime Date { get; set; }
    public string VaccineTypes { get; set; } = string.Empty;
    public decimal ApplicationValue { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalCost { get; set; }
}