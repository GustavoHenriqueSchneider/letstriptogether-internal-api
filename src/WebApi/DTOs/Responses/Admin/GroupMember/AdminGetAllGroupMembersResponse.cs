namespace WebApi.DTOs.Responses.Admin.GroupMember;

public class AdminGetAllGroupMembersResponse : PaginatedResponse<AdminGetAllGroupMembersResponseData>
{
}

public class AdminGetAllGroupMembersResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
