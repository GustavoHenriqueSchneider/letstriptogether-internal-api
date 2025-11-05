namespace LetsTripTogether.InternalApi.Application.UseCases.Admin.AdminGroupMember.Query.AdminGetGroupMemberById;

public class AdminGetGroupMemberByIdResponse
{
    public Guid UserId { get; init; }
    public bool IsOwner { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
