namespace FarmSystemProject.DTOs.HealthMonitoringDTO;
public class VaccinationDTO
{
    public int Id { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string VaccineType { get; set; }
    public decimal ApplicationValue { get; set; }
    public int ApplicationQuantity { get; set; }
    public int LotId { get; set; }
}
