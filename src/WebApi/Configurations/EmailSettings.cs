namespace WebApi.Configurations;

public class EmailSettings
{
    public string SmtpServer { get; init; } = null!;
    public int Port { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public bool EnableSsl { get; init; }
}
