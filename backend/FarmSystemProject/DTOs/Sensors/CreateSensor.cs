using FarmSystemProject.Models.Sensors;
using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Sensors;

public class CreateSensor
{
    [Required(ErrorMessage = "O Endereço MAC é obrigatório.")]
    public string MacAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Tipo de sensor é obrigatório.")]
    public SensorType Type { get; set; }

    [Required(ErrorMessage = "O ID do Lote é obrigatório.")]
    public int LotId { get; set; }
}
