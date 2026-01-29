namespace FarmSystemProject.DTOs.HealthMonitoringDTO;

public class MortalityResponse
{
    public int Id { get; set; }
    public DateTime DateDeath { get; set; }
    public int DeathQuantity { get; set; }
    public int CutQuantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int LotId { get; set; }
}