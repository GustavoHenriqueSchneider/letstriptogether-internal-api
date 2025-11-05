namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;

public class LoginResponse
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
