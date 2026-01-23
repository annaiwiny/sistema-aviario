using FarmSystemProject.Models.Farm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.HealthMonitoring;
public class Mortality
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime DateDeath { get; set; }
    [Required]
    public int DeathQuantity { get; set; }
    [Required]
    public int CutQuantity { get; set; }
    [Required, MaxLength(255)]
    public string Reason { get; set; }
    public int LotId { get; set; }
    [ForeignKey("LotId")]
    public Lot Lot { get; set; }
}
