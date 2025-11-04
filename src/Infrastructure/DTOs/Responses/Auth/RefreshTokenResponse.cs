namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Auth;

public class RefreshTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
