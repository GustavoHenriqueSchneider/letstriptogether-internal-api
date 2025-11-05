using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetGroupById;

public class GetGroupByIdQuery : IRequest<GetGroupByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
