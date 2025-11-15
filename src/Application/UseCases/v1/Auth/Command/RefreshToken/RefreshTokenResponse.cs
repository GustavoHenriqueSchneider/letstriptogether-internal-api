namespace Application.UseCases.v1.Auth.Command.RefreshToken;

public class RefreshTokenResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
