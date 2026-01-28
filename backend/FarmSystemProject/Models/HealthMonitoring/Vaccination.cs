using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.HealthMonitoring;
public class Vaccination
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime ApplicationDate { get; set; }
    [Required, MaxLength(100)]
    public string VaccineType { get; set; }
    [Required]
    public decimal ApplicationValue { get; set; }
    [Required]
    public int ApplicationQuantity { get; set; }
    [Required]
    public int LotId { get; set; }
    [ForeignKey("LotId")]
    public Lot Lot { get; set; }
}
