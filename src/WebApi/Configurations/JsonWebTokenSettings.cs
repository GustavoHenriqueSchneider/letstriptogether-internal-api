﻿namespace WebApi.Configurations;

public class JsonWebTokenSettings
{
    public string Audience { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string SecretKey { get; init; } = null!;
    public int AccessTokenValidityInMinutes { get; init; }
    public int RefreshTokenValidityInMinutes { get; init; }
    public int ResetPasswordTokenValidityInMinutes { get; init; }
}
