using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Sensors;

public class Sensor
{
    [Key]
    public int Id { get; set; }

    [Required]
    public SensorType Type { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public string MacAddress { get; set; } = string.Empty;

    [Required]
    public string LocalId { get; set; } = string.Empty;

    [Required]
    public int LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;

    public ICollection<SensorReading> Readings { get; set; } = [];
}
