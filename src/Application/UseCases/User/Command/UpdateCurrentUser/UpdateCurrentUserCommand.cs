using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.UpdateCurrentUser;

public class UpdateCurrentUserCommand : IRequest
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
}
