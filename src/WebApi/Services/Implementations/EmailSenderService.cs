using System.Net.Mail;
using WebApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace WebApi.Services.Implementations;

public class EmailSenderService(SmtpClient smtpClient, ILogger<EmailSenderService> logger) : IEmailSenderService
{
    public async Task SendAsync(string to, string subject, string body)
    {
		try
		{
        var mailMessage = new MailMessage("no-reply@letstriptogether.com", to, subject, body);
			logger.LogInformation("Sending email to {To}", to);

        await smtpClient.SendMailAsync(mailMessage);

			logger.LogInformation("Email sent successfully to {To}", to);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error sending email to {To}", to);
			throw;
		}
    }
}
