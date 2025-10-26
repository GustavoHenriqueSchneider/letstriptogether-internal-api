using WebApi.DTOs.Responses;

namespace WebApi.DTOs.Requests.GroupMember;

public class GetGroupMemberAllDestinationVotesByIdResponse
    : PaginatedResponse<GetGroupMemberAllDestinationVotesByIdResponseData>
{
}

public class GetGroupMemberAllDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
