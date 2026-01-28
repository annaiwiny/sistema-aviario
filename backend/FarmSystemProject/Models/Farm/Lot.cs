using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Farm;
public class Lot
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime AccommodationDate { get; set; }
    [Required]
    public int FarmId { get; set; }
    [ForeignKey("FarmId")]
    public Farm Farm { get; set; }
    public ICollection<LotItem> Items { get; set; } = new List<LotItem>();
}
