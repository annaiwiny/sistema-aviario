namespace FarmSystemProject.DTOs.Sensors;

public class Esp32Reading
{
    public string LocalId { get; set; } = string.Empty;
    public float Value { get; set; }
}