namespace Application.Common.Interfaces.Services;

public interface IEmailSenderService
{
    Task SendAsync(string to, string templateName, Dictionary<string, string> templateData, CancellationToken cancellationToken);
}
