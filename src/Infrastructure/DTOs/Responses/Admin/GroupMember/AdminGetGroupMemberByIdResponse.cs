namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.Admin.GroupMember;

public class AdminGetGroupMemberByIdResponse
{
    public Guid UserId { get; init; }
    public bool IsOwner { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
