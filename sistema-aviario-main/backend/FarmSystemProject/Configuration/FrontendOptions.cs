using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Configuration;

/// <summary>
/// URL pública do frontend, usada para montar o link de redefinição de senha enviado por e-mail.
/// </summary>
public class FrontendOptions
{
    public const string SectionName = "Frontend";

    [Required(ErrorMessage = "Frontend__BaseUrl não configurada.")]
    [Url(ErrorMessage = "Frontend__BaseUrl precisa ser uma URL absoluta (ex.: https://app.seudominio.com).")]
    public string BaseUrl { get; set; } = string.Empty;
}
