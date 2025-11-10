namespace Application.UseCases.Admin.AdminGroup.Query.AdminGetAllGroups;

public class AdminGetAllGroupsResponse
{
    public IEnumerable<AdminGetAllGroupsResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllGroupsResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
