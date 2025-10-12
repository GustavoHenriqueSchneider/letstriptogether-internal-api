using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupMemberDestinationVoteRepository 
    : BaseRepository<GroupMemberDestinationVote> , IGroupMemberDestinationVoteRepository
{
    public GroupMemberDestinationVoteRepository(AppDbContext context) : base(context) { }
}
