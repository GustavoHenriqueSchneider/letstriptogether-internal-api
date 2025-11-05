using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
