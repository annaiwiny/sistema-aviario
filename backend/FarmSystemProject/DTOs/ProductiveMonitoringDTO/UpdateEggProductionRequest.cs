using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.ProductiveMonitoringDTO;

public class UpdateEggProductionRequest
{
    [Required(ErrorMessage = "A data da coleta é obrigatória.")]
    public DateTime ProductionDate { get; set; }

    // Aceita zero, ao contrário do cadastro: corrigir um lançamento errado para
    // "não houve coleta nesse dia" é justamente um dos casos desta rota.
    [Required(ErrorMessage = "A quantidade é obrigatória.")]
    [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
    public int Quantity { get; set; }
}
