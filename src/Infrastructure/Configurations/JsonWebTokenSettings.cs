namespace Infrastructure.Configurations;

public class JsonWebTokenSettings
{
    public string Issuer { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public int AccessTokenValidityInMinutes { get; init; }
    public int RefreshTokenValidityInMinutes { get; init; }
    public int ResetPasswordTokenValidityInMinutes { get; init; }
    public int InvitationTokenValidityInMinutes { get; init; }
}
