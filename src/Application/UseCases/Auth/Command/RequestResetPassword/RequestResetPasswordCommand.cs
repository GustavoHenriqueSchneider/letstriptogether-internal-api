using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.RequestResetPassword;

public class RequestResetPasswordCommand : IRequest
{
    public string Email { get; init; } = null!;
}
