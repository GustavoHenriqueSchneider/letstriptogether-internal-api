namespace Application.UseCases.v1.Auth.Command.Login;

public class LoginResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
