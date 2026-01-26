using FarmSystemProject.Interfaces;
using System.Net;
using System.Net.Mail;

namespace FarmSystemProject.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("Email");

        var smtpClient = new SmtpClient
        {
            Host = emailSettings["Host"]!,
            Port = int.Parse(emailSettings["Port"]!),
            EnableSsl = bool.Parse(emailSettings["EnableSsl"]!),
            Credentials = new NetworkCredential(
                emailSettings["User"],
                emailSettings["Password"]
            )
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(emailSettings["User"]!, emailSettings["From"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        mailMessage.To.Add(to);

        await smtpClient.SendMailAsync(mailMessage);
    }
}