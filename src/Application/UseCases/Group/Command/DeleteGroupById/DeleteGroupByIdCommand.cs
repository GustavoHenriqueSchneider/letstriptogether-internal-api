using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Command.DeleteGroupById;

public class DeleteGroupByIdCommand : IRequest
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
