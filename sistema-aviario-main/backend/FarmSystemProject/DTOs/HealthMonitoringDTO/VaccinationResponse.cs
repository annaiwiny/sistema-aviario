namespace FarmSystemProject.DTOs.HealthMonitoringDTO;

public class VaccinationResponse
{
    public int Id { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string VaccineType { get; set; } = string.Empty;
    public decimal ApplicationValue { get; set; }
    public int ApplicationQuantity { get; set; }
    public decimal TotalCost { get; set; }
    public int LotId { get; set; }
}
