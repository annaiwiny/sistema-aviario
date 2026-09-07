using FarmSystemProject.Models.Farms;
using FarmSystemProject.Models.Lots;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmSystemProject.Models.Notifications;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public NotificationType Type { get; set; }

    [Required, MaxLength(255)]
    public string Title { get; set; } = null!;

    [Required, MaxLength(500)]
    public string Description { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }

    // Data do evento que originou o alerta. Usada para não repetir o mesmo alerta no mesmo dia.
    [Required]
    public DateTime ReferenceDate { get; set; }

    [Required]
    public bool IsRead { get; set; }

    [Required]
    public int FarmId { get; set; }

    [ForeignKey("FarmId")]
    public Farm Farm { get; set; } = null!;

    public int? LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot? Lot { get; set; }
}
