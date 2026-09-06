using FarmSystemProject.Configuration;
using FarmSystemProject.Exceptions;
using FarmSystemProject.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace FarmSystemProject.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _email;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _email = emailOptions.Value;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        if (!_email.IsConfigured)
            throw new BusinessException("Envio de e-mail indisponível: SMTP não configurado no servidor.");

        using var smtpClient = new SmtpClient
        {
            Host = _email.Host,
            Port = _email.Port,
            EnableSsl = _email.EnableSsl,
            Credentials = new NetworkCredential(_email.User, _email.Password)
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_email.User, _email.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
}
