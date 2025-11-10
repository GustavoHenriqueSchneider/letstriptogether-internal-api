namespace Application.UseCases.GroupMatch.Query.GetAllGroupMatchesById;

public class GetAllGroupMatchesByIdResponse
{
    public IEnumerable<GetAllGroupMatchesByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class GetAllGroupMatchesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
