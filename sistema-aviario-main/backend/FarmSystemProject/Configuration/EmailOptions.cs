using System.ComponentModel.DataAnnotations;

namespace FarmSystemProject.Configuration;

/// <summary>
/// Credenciais SMTP usadas no envio de e-mail de recuperação de senha.
/// Nunca devem ser versionadas: em produção vêm de variáveis de ambiente (Email__Password, ...).
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    [Required(ErrorMessage = "Email__Host não configurado.")]
    public string Host { get; set; } = string.Empty;

    [Range(1, 65535, ErrorMessage = "Email__Port inválida.")]
    public int Port { get; set; } = 587;

    public bool EnableSsl { get; set; } = true;

    [Required(ErrorMessage = "Email__User não configurado.")]
    [EmailAddress(ErrorMessage = "Email__User precisa ser um endereço de e-mail válido.")]
    public string User { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email__Password não configurada.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email__From não configurado.")]
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// True quando há SMTP suficiente para tentar um envio. Em Development permitimos
    /// rodar sem SMTP; o erro só aparece se alguém realmente pedir "esqueci minha senha".
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Host) &&
        !string.IsNullOrWhiteSpace(User) &&
        !string.IsNullOrWhiteSpace(Password);
}
