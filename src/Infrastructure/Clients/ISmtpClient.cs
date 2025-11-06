using System.Net.Mail;

namespace LetsTripTogether.InternalApi.Infrastructure.Clients;

public interface ISmtpClient
{
    Task SendMailAsync(MailMessage message, CancellationToken cancellationToken);
}
