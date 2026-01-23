using FarmSystemProject.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Farm;
public class Farm
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [Required]
    public int OwnerId { get; set; }
    [ForeignKey("OwnerId")]
    public User Owner { get; set; }
}
