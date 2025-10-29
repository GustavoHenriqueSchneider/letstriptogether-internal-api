namespace WebApi.DTOs.Responses.GroupMember;

public class GetAllGroupMembersResponse : PaginatedResponse<GetAllGroupMembersResponseData>
{
}

public class GetAllGroupMembersResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
