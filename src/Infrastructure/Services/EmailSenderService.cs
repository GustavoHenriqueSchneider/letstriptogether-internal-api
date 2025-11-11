using System.Net.Mail;
using System.Text;
using Application.Common.Interfaces.Services;
using Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailSenderService(
    string from,
    ISmtpClient smtpClient, 
    IEmailTemplateService emailTemplateService,
    ILogger<EmailSenderService> logger) : IEmailSenderService
{
    public async Task SendAsync(string to, string templateName, Dictionary<string, string> templateData, 
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Preparando envio de email para {To} usando template {TemplateName}", to, templateName);
            
            var (subject, htmlBody) = await emailTemplateService.GetTemplateAsync(templateName, templateData);

            var mailMessage = new MailMessage(from, to, subject, htmlBody)
            {
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };
            
            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            
            logger.LogInformation("Email enviado com sucesso para {To} com assunto {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao enviar email para {To} usando template {TemplateName}", to, templateName);
            throw;
        }
    }
}
