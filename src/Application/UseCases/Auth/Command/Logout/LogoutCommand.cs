using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.Logout;

public class LogoutCommand : IRequest
{
    public Guid UserId { get; init; }
}
