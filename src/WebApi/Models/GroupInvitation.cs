namespace WebApi.Models;

public class GroupInvitation : TrackableEntity
{
    public DateTime ExpirationDate { get; init; }
    public List<UserGroupInvitation> AnsweredBy { get; init; } = [];
}
