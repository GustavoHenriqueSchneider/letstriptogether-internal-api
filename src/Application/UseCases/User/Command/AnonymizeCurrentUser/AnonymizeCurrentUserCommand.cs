using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.AnonymizeCurrentUser;

public class AnonymizeCurrentUserCommand : IRequest
{
    public Guid UserId { get; init; }
}
