using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.User.Command.DeleteCurrentUser;

public class DeleteCurrentUserCommand : IRequest
{
    public Guid UserId { get; init; }
}
