using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Clients;

public class SmtpClientWrapper : ISmtpClient
{
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<SmtpClientWrapper> _logger;

    public SmtpClientWrapper(SmtpClient smtpClient, ILogger<SmtpClientWrapper> logger)
    {
        _smtpClient = smtpClient;
        _logger = logger;
    }

    public async Task SendMailAsync(MailMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _smtpClient.SendMailAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email via SMTP para {To}", message.To.ToString());
            throw;
        }
    }
}
