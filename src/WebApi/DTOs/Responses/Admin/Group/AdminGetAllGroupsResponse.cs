namespace WebApi.DTOs.Responses.Admin.Group;

public class AdminGetAllGroupsResponse : PaginatedResponse<AdminGetAllGroupsResponseData>
{
}

public class AdminGetAllGroupsResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
