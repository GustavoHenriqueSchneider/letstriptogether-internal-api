namespace WebApi.DTOs.Responses.GroupMember;

public class GetOtherGroupMembersByIdResponse : PaginatedResponse<GetOtherGroupMembersByIdResponseData>
{
}

public class GetOtherGroupMembersByIdResponseData
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
}
