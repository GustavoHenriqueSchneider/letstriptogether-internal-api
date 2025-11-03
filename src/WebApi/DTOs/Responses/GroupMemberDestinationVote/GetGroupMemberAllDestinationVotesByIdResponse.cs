namespace WebApi.DTOs.Responses.GroupMemberDestinationVote;

public class GetGroupMemberAllDestinationVotesByIdResponse
    : PaginatedResponse<GetGroupMemberAllDestinationVotesByIdResponseData>
{
}

public class GetGroupMemberAllDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
