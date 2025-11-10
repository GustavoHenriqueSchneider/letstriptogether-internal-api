namespace Application.UseCases.GroupDestinationVote.Query.GetGroupMemberAllDestinationVotesById;

public class GetGroupMemberAllDestinationVotesByIdResponse
{
    public IEnumerable<GetGroupMemberAllDestinationVotesByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class GetGroupMemberAllDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
