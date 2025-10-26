namespace WebApi.DTOs.Responses.Admin.User;

public class AdminGetAllUsersResponse : PaginatedResponse<AdminGetAllUsersResponseData>
{
}

public class AdminGetAllUsersResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
