using System.Net.Mail;

namespace Infrastructure.Clients;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientWrapper(SmtpClient smtpClient)
    {
        _smtpClient = smtpClient;
    }

    public Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
    {
        return _smtpClient.SendMailAsync(message, cancellationToken);
    }
}
