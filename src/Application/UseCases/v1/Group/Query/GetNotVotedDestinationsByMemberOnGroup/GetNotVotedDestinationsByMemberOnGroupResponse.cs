namespace Application.UseCases.v1.Group.Query.GetNotVotedDestinationsByMemberOnGroup;

public class GetNotVotedDestinationsByMemberOnGroupResponse
{
    public IEnumerable<GetNotVotedDestinationsByMemberOnGroupResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class GetNotVotedDestinationsByMemberOnGroupResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
