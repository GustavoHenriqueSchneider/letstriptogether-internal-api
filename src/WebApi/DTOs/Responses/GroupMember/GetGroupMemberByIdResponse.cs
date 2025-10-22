namespace WebApi.DTOs.Responses.GroupMember;

public class GetGroupMemberByIdResponse
{
    public Guid UserId { get; init; }
    public bool IsOwner { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
