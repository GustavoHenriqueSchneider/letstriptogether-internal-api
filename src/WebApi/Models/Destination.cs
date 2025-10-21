namespace WebApi.Models;

public class Destination : TrackableEntity
{
    public string Address { get; init; } = null!;
    // TODO: fazer listas readonly
    public List<GroupMatch> GroupMatches { get; init; } = [];
    public List<GroupMemberDestinationVote> GroupMemberVotes { get; init; } = [];
    public List<string> Categories { get; init; } = [];
}
