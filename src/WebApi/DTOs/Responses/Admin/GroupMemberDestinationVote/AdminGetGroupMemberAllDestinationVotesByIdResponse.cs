namespace WebApi.DTOs.Responses.GroupMemberDestinationVote;

public class AdminGetGroupMemberAllDestinationVotesByIdResponse
    : PaginatedResponse<AdminGetAllGroupDestinationVotesByIdResponseData>
{
}

public class AdminGetGroupMemberAllDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
