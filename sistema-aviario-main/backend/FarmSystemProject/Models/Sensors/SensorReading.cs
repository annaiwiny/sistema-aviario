using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Sensors;

public class SensorReading
{
    [Key]
    public int Id { get; set; }

    [Required]
    public float Value { get; set; }

    [Required]
    public DateTime MeasuredAt { get; set; }

    [Required]
    public int SensorId { get; set; }

    [ForeignKey("SensorId")]
    public Sensor Sensor { get; set; } = null!;
}
