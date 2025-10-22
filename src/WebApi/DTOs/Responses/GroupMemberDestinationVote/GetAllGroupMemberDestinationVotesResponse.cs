namespace WebApi.DTOs.Responses.GroupMemberDestinationVote;

public class GetAllGroupMemberDestinationVotesResponse : PaginatedResponse<GetAllGroupMemberDestinationVotesResponseData>
{
}

public class GetAllGroupMemberDestinationVotesResponseData
{
    public Guid Id { get; init; }
    public Guid GroupMemberId { get; init; }
    public string GroupMemberName { get; init; } = null!;
    public Guid DestinationId { get; init; }
    public string DestinationAddress { get; init; } = null!;
    public bool IsApproved { get; init; }
    public DateTime CreatedAt { get; init; }
}
