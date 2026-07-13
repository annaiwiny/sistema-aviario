namespace FarmSystemProject.DTOs.Sensors;

public class SensorSummary
{
    public string Type { get; set; } = null!;
    public string? Value { get; set; }
    public DateTime? MeasuredAt { get; set; }
    public string Status { get; set; } = null!;
}