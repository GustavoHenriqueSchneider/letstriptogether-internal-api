namespace Application.Common.Interfaces.Services;

public interface IEmailTemplateService
{
    Task<(string subject, string htmlBody)> GetTemplateAsync(string templateName, Dictionary<string, string> data);
}
