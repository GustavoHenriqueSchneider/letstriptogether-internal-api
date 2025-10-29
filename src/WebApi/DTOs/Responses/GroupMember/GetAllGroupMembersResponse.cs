namespace WebApi.DTOs.Responses.GroupMember;

public class GetAllGroupMembersByIdResponse : PaginatedResponse<GetAllGroupMembersByIdResponseData>
{
}

public class GetAllGroupMembersByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
