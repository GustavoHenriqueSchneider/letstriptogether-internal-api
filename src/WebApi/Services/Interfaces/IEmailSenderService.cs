namespace WebApi.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendAsync(string to, string subject, string body);
}
