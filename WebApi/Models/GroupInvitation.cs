namespace WebApi.Models
{
    public class GroupInvitation : TrackableEntity
    {
        public Group Group { get; set; } = null!;
        public Guid GroupId { get; set; } 
        public List<User> AcceptedBy { get; set; } = new();
        public DateTime ExpirationDate { get; set; }
    }
}
