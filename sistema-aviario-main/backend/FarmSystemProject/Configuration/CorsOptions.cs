namespace FarmSystemProject.Configuration;

/// <summary>
/// Origens liberadas para chamadas cross-origin.
/// Vazio significa "somente mesma origem" — que é o caso do deploy padrão,
/// onde o nginx do frontend faz proxy reverso de /api para a API.
/// </summary>
public class CorsOptions
{
    public const string SectionName = "Cors";

    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
