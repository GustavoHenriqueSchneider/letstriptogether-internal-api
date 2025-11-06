using System.Net.Mail;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Infrastructure.Clients;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

public class EmailSenderService(ISmtpClient smtpClient) : IEmailSenderService
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var mailMessage = new MailMessage("no-reply@letstriptogether.com", to, subject, body);
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
