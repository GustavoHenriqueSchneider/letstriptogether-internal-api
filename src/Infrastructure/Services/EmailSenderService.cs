using System.Net.Mail;
using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

public class EmailSenderService(SmtpClient smtpClient) : IEmailSenderService
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        // TODO: tirar valor hard coded
        var mailMessage = new MailMessage("no-reply@letstriptogether.com", to, subject, body);
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
