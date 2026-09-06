using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Configuration;

/// <summary>
/// Chave compartilhada usada pelo ESP32 para autenticar nos endpoints anônimos de sensores.
/// </summary>
public class Esp32Options
{
    public const string SectionName = "Esp32Config";

    [Required(ErrorMessage = "Esp32Config__SecretKey não configurada.")]
    [MinLength(24, ErrorMessage = "Esp32Config__SecretKey precisa ter no mínimo 24 caracteres.")]
    public string SecretKey { get; set; } = string.Empty;
}
