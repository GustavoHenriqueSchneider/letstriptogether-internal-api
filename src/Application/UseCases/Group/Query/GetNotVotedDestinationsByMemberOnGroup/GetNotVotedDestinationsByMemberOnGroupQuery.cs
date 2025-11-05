using MediatR;

namespace LetsTripTogether.InternalApi.Application.UseCases.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupQuery : IRequest<GetNotVotedDestinationsByMemberOnGroupResponse>
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
