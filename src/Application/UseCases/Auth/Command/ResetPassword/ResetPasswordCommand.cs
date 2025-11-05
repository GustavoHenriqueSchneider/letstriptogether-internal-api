using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ResetPassword;

public class ResetPasswordCommand : IRequest
{
    public string Password { get; init; } = null!;
    public Guid UserId { get; init; }
    public string BearerToken { get; init; } = null!;
}
