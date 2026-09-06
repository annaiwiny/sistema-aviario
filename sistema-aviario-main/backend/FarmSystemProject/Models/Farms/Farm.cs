using FarmSystemProject.Models.Users;
using FarmSystemProject.Models.Lots;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Farms;

[Index(nameof(OwnerId), IsUnique = true)]
public class Farm
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [Required]
    public int OwnerId { get; set; }

    [ForeignKey("OwnerId")]
    public User Owner { get; set; } = null!;

    public ICollection<Lot> Lots { get; set; } = [];
}