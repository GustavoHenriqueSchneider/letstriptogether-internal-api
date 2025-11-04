namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.GroupMemberDestinationVote;

public class AdminGetAllGroupDestinationVotesByIdResponse 
    : PaginatedResponse<AdminGetAllGroupDestinationVotesByIdResponseData>
{
}

public class AdminGetAllGroupDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
