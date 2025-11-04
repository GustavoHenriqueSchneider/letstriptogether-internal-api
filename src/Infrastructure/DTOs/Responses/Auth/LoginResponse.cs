namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Auth;

public class LoginResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
