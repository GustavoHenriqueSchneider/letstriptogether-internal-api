namespace WebApi.Models;

public class GroupInvitation : TrackableEntity
{
    public Guid GroupId { get; init; }
    public Group Group { get; init; } = null!;
    public DateTime ExpirationDate { get; init; }
    public List<UserGroupInvitation> AnsweredBy { get; init; } = [];
    private GroupInvitation() { }
}