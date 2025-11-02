namespace WebApi.DTOs.Responses.Admin.GroupMember;

public class AdminGetAllGroupMembersByIdResponse : PaginatedResponse<AdminGetAllGroupMembersByIdResponseData>
{
}

public class AdminGetAllGroupMembersByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
