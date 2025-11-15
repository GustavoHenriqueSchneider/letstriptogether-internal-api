namespace Application.UseCases.v1.Group.Query.GetAllGroups;

public class GetAllGroupsResponse
{
    public IEnumerable<GetAllGroupsResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class GetAllGroupsResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
