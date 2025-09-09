namespace WebApi.Models
{
    public class GroupMember : TrackableEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Group Group { get; set; } = null!;
        public Guid GroupId { get; set; }
    }
}
