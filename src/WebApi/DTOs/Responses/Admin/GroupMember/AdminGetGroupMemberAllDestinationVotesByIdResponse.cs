using WebApi.DTOs.Responses.GroupMemberDestinationVote;

namespace WebApi.DTOs.Responses.Admin.GroupMember;

public class AdminGetGroupMemberAllDestinationVotesByIdResponse
    : PaginatedResponse<AdminGetGroupMemberAllDestinationVotesByIdResponseData>
{
}

public class AdminGetGroupMemberAllDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
