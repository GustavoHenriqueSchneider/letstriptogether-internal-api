using MediatR;

namespace Application.UseCases.Auth.Command.RequestResetPassword;

public class RequestResetPasswordCommand : IRequest
{
    public string Email { get; init; } = null!;
}
