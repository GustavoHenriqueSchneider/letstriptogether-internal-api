namespace LetsTripTogether.InternalApi.Infrastructure.DTOs.Responses.GroupMember;

public class GetGroupMemberByIdResponse
{
    public string Name { get; init; } = null!;
    public bool IsOwner { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
