using System.Net.Mail;
using System.Text;
using Application.Common.Interfaces.Services;
using Infrastructure.Clients;

namespace Infrastructure.Services;

public class EmailSenderService(
    string from,
    ISmtpClient smtpClient, 
    IEmailTemplateService emailTemplateService) : IEmailSenderService
{
    public async Task SendAsync(string to, string templateName, Dictionary<string, string> templateData, 
        CancellationToken cancellationToken)
    {
        var (subject, htmlBody) = await emailTemplateService.GetTemplateAsync(templateName, templateData);

        var mailMessage = new MailMessage(from, to, subject, htmlBody)
        {
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };
        
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
