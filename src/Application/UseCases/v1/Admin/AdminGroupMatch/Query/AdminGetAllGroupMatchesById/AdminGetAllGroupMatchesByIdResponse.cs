namespace Application.UseCases.v1.Admin.AdminGroupMatch.Query.AdminGetAllGroupMatchesById;

public class AdminGetAllGroupMatchesByIdResponse
{
    public IEnumerable<AdminGetAllGroupMatchesByIdResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllGroupMatchesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
