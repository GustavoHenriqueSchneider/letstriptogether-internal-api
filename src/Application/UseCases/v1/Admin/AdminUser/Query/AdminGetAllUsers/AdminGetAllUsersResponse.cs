namespace Application.UseCases.v1.Admin.AdminUser.Query.AdminGetAllUsers;

public class AdminGetAllUsersResponse
{
    public IEnumerable<AdminGetAllUsersResponseData> Data { get; init; } = [];
    public int Hits { get; init; }
}

public class AdminGetAllUsersResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
