using System.Net.Mail;
using Application.Common.Interfaces.Services;
using Infrastructure.Clients;

namespace Infrastructure.Services;

public class EmailSenderService(ISmtpClient smtpClient) : IEmailSenderService
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var mailMessage = new MailMessage("no-reply@letstriptogether.com", to, subject, body);
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
