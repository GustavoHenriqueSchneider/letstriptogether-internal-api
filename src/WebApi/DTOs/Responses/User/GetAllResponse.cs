namespace WebApi.DTOs.Responses.User;

public class GetAllResponse
{
    public IEnumerable<GetAllUsersResponse> Users { get; init; } = [];
    public int Hits { get; init; }
}

public class GetAllUsersResponse
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}