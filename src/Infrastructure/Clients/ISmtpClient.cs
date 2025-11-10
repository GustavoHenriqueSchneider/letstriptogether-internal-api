using System.Net.Mail;

namespace Infrastructure.Clients;

public interface ISmtpClient
{
    Task SendMailAsync(MailMessage message, CancellationToken cancellationToken);
}
