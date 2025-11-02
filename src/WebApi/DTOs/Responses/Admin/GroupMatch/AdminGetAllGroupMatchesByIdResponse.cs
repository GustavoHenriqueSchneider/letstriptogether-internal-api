namespace WebApi.DTOs.Responses.Admin.GroupMatch;

public class AdminGetAllGroupMatchesByIdResponse
    : PaginatedResponse<AdminGetAllGroupMatchesByIdResponseData>
{
}

public class AdminGetAllGroupMatchesByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
