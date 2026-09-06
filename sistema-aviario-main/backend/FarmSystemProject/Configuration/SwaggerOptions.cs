namespace FarmSystemProject.Configuration;

/// <summary>
/// Swagger é habilitado automaticamente em Development.
/// Em produção só liga se Swagger__Enabled=true for definido explicitamente.
/// </summary>
public class SwaggerOptions
{
    public const string SectionName = "Swagger";

    public bool? Enabled { get; set; }
}
