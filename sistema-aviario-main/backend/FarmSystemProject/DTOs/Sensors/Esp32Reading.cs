using FarmSystemProject.Models.Sensors;

namespace FarmSystemProject.DTOs.Sensors;

public class Esp32Reading
{
    public SensorType Type { get; set; }
    public float Value { get; set; }
}