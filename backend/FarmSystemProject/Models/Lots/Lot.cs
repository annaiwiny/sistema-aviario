using FarmSystemProject.Models.Farms;
using FarmSystemProject.Models.Sensors;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Lots;
public class Lot
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime AccommodationDate { get; set; }

    [Required]
    public int FarmId { get; set; }

    [ForeignKey("FarmId")]
    public Farm Farm { get; set; } = null!;

    public ICollection<Lineage> Lineages { get; set; } = [];

    public ICollection<Sensor> Sensors { get; set; } = [];
}