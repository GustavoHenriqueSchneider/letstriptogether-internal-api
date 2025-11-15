namespace Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberAllDestinationVotesById;

public class AdminGetGroupMemberAllDestinationVotesByIdResponse
{
    public IEnumerable<AdminGetGroupMemberAllDestinationVotesByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetGroupMemberAllDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
