namespace Application.Common.Interfaces.Services;

public interface IEmailSenderService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken);
}
