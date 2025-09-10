namespace WebApi.Models;

public class Destination : TrackableEntity
{
    public string Address { get; init; } = null!;
    public List<GroupMatch> GroupMatches { get; init; } = [];
    public List<GroupMemberDestinationVote> GroupMemberVotes { get; init; } = [];
    public List<string> Categories { get; init; } = [];
}
