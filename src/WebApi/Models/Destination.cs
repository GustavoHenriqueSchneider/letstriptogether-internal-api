namespace WebApi.Models;

public class Destination : TrackableEntity
{
    public string Address { get; init; } = null!;

    private readonly List<GroupMatch> _groupMatches = [];
    public IReadOnlyCollection<GroupMatch> GroupMatches => _groupMatches.AsReadOnly();

    private readonly List<GroupMemberDestinationVote> _groupMemberVotes = [];
    public IReadOnlyCollection<GroupMemberDestinationVote> GroupMemberVotes => _groupMemberVotes.AsReadOnly();

    private readonly List<string> _categories = [];
    public IReadOnlyCollection<string> Categories => _categories.AsReadOnly();
    
    private Destination() { }

    public Destination(string address, List<string> categories)
    {
        Address = address;
        _categories = categories;
    }
}
