using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Models.Farm;
public class Race
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(30)]
    public string Name { get; set; }
}
