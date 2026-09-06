using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.HealthMonitoringDTO;

public class CreateVaccinationRequest
{
    [Required(ErrorMessage = "A data da aplicação é obrigatória.")]
    public DateTime ApplicationDate { get; set; }

    [Required(ErrorMessage = "O tipo da vacina é obrigatório.")]
    [MaxLength(255, ErrorMessage = "O tipo da vacina deve ter no máximo 255 caracteres.")]
    public string VaccineType { get; set; } = string.Empty;

    [Required(ErrorMessage = "O valor individual é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal ApplicationValue { get; set; }

    [Required(ErrorMessage = "A quantidade de aplicações é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int ApplicationQuantity { get; set; }
}