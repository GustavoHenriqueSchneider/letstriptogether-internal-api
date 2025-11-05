using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.LeaveGroupById;

public class LeaveGroupByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
