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

    [Required, MaxLength(255)]
    public string VaccineType { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ApplicationValue { get; set; }

    [Required]
    public int ApplicationQuantity { get; set; }

    // Atributo auxiliar (Não cria coluna no banco)
    [NotMapped]
    public decimal TotalCost => ApplicationValue * ApplicationQuantity;

    [Required]
    public int LotId { get; set; }

    [ForeignKey("LotId")]
    public Lot Lot { get; set; } = null!;
}