namespace Application.UseCases.Auth.Command.RefreshToken;

public class RefreshTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
