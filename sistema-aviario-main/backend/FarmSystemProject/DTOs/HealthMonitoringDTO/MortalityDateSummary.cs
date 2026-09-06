namespace FarmSystemProject.DTOs.HealthMonitoringDTO;

public class MortalityDateSummary
{
    public DateTime Date { get; set; }
    public int TotalDeaths { get; set; }
    public int TotalCuts { get; set; }
    public string Motives { get; set; } = string.Empty;
    public decimal MortalityRate { get; set; }
}
