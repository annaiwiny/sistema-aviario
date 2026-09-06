using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Configuration;

/// <summary>
/// Configuração de assinatura dos tokens JWT.
/// Origem esperada em produção: variáveis de ambiente (Jwt__Key, Jwt__Issuer, ...).
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required(ErrorMessage = "Jwt__Key não configurada.")]
    [MinLength(32, ErrorMessage = "Jwt__Key precisa ter no mínimo 32 caracteres.")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jwt__Issuer não configurado.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jwt__Audience não configurado.")]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440, ErrorMessage = "Jwt__ExpirationMinutes precisa estar entre 1 e 1440.")]
    public int ExpirationMinutes { get; set; } = 60;
}
