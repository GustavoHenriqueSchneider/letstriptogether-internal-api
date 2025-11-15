namespace Application.UseCases.Admin.AdminGroupDestinationVote.Query.AdminGetAllGroupDestinationVotesById;

public class AdminGetAllGroupDestinationVotesByIdResponse
{
    public IEnumerable<AdminGetAllGroupDestinationVotesByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllGroupDestinationVotesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
