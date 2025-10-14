namespace WebApi.DTOs.Responses.User;

public class GetAllUsersResponse : PaginatedResponse<GetAllUsersResponseData>
{
}

public class GetAllUsersResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
