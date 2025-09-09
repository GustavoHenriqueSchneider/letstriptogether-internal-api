namespace WebApi.Models
{
    public class User : TrackableEntity
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public List<GroupMember> GroupMemberships { get; set; } = new();
        public List<GroupInvitation> AcceptedInvitation { get; set; } = new();
    }
}
