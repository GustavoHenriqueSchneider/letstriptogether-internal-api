namespace WebApi.DTOs.Requests.GroupMember;

public class CreateGroupMemberRequest
{
    public Guid GroupId { get; init; }
    public Guid UserId { get; init; }
}
