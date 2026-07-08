using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.ProductiveMonitoringDTO;

public class CreateEggProductionRequest
{
    [Required(ErrorMessage = "A data da coleta é obrigatória.")]
    public DateTime ProductionDate { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantity { get; set; }
}