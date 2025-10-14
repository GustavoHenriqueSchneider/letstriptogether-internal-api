namespace WebApi.DTOs.Responses.Group;

public class GetAllGroupsResponse : PaginatedResponse<GetAllGroupsResponseData>
{
}

public class GetAllGroupsResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
