namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupMatch;

public class GetAllGroupMatchesByIdResponse
    : PaginatedResponse<GetAllGroupMatchesByIdResponseData>
{
}

public class GetAllGroupMatchesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
