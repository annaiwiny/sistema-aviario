using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.HealthMonitoringDTO;

public class CreateMortalityRequest
{
    [Required(ErrorMessage = "A data é obrigatória.")]
    public DateTime DateDeath { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade de mortes não pode ser negativa.")]
    public int DeathQuantity { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "A quantidade de corte não pode ser negativa.")]
    public int CutQuantity { get; set; }

    [Required(ErrorMessage = "O motivo é obrigatório.")]
    [MaxLength(255, ErrorMessage = "O motivo deve ter no máximo 255 caracteres.")]
    public string Reason { get; set; } = string.Empty;
}