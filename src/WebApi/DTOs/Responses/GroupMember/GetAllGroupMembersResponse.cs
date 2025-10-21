namespace WebApi.DTOs.Responses.GroupMember;

public class GetAllGroupMembersResponse : PaginatedResponse<GetAllGroupMembersResponseData>
{
}

public class GetAllGroupMembersResponseData
{
    public Guid Id { get; init; }
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
    public bool IsOwner { get; init; }
    public DateTime CreatedAt { get; init; }
}
