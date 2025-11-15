using MediatR;

namespace Application.UseCases.v1.Auth.Command.RequestResetPassword;

public class RequestResetPasswordCommand : IRequest
{
    public string Email { get; init; } = null!;
}
