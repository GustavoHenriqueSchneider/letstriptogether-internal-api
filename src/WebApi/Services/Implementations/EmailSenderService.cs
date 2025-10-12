using System.Net.Mail;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class EmailSenderService(SmtpClient smtpClient) : IEmailSenderService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        // TODO: tirar valor hard coded
        var mailMessage = new MailMessage("no-reply@letstriptogether.com", to, subject, body);
        await smtpClient.SendMailAsync(mailMessage);
    }
}
