namespace WebApi.DTOs.Responses.GroupMatch;

public class GetAllGroupMatchesResponse
{
    public IEnumerable<GetAllGroupMatchesResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class GetAllGroupMatchesResponseData
{
    public Guid Id { get; init; }
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = null!;
    public Guid DestinationId { get; init; }
    public string DestinationAddress { get; init; } = null!;
    public List<string> DestinationCategories { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}
