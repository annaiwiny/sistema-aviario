namespace FarmSystemProject.DTOs.HealthMonitoringDTO;
public class MortalityDTO
{
    public int Id { get; set; }
    public DateTime DateDeath { get; set; }
    public int DeathQuantity { get; set; }
    public int CutQuantity { get; set; }
    public string Reason { get; set; }
    public int LotId { get; set; }
}
