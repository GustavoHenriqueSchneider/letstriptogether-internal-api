using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.UpdateCurrentUser;

public record UpdateCurrentUserCommand : IRequest
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
}
