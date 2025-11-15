using MediatR;

namespace Application.UseCases.v1.Auth.Command.Login;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}
