using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.DTOs.Sensors;

public class Esp32Payload
{
    [Required(ErrorMessage = "Endereço MAC é obrigatório")]
    public string MacAddress { get; set; } = null!;

    [Required(ErrorMessage = "As leituras são obrigatório")]
    public List<Esp32Reading> Readings { get; set; } = [];
}