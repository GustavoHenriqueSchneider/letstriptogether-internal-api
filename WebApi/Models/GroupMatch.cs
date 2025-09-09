namespace WebApi.Models
{
    public class GroupMatch : TrackableEntity
    {
        public Guid GroupId { get; set; }
        public Group Group { get; set; } = null!;
        public Guid DestinationId { get; set; }
        public Destination Destination { get; set; } = null!;
    }
}
