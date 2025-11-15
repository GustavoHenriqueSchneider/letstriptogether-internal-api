using MediatR;

namespace Application.UseCases.v1.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

public class AdminGetGroupMemberAllDestinationVotesByIdQuery : IRequest<AdminGetGroupMemberAllDestinationVotesByIdResponse>
{
    public Guid GroupId { get; init; }
    public Guid MemberId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
