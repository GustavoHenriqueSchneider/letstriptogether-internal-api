namespace WebApi.DTOs.Responses.GroupMember;

public class GetGroupMemberByIdResponse
{
    public Guid Id { get; init; }
    public Guid GroupId { get; init; }
    public string GroupName { get; init; } = null!;
    public Guid UserId { get; init; }
    public string UserName { get; init; } = null!;
    public string UserEmail { get; init; } = null!;
    public bool IsOwner { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
