namespace WebApi.Models;

public class GroupInvitation : TrackableEntity
{
    public string Token { get; init; } = null!;
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;
    public DateTime ExpirationDate { get; init; }
    public List<UserGroupInvitation> AnsweredBy { get; init; } = [];
    private GroupInvitation()
    {
        Token = string.Empty;
        GroupId = Guid.Empty;
        ExpirationDate = DateTime.MinValue;
    }
}