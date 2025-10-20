namespace WebApi.Configurations;

public class GeoapifyApiSettings
{
    public string ApiKey { get; init; } = null!;
    public string BaseUrl { get; init; } = null!;
    public string Categories { get; init; } = null!;
    public string Filter { get; init; } = null!;
    public string Lang { get; init; } = null!;
}
