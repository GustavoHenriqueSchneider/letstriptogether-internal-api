using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations
{
    public class GroupMemberDestinationVoteRepository : BaseRepository<GroupMemberDestinationVote> , IGroupMemberDestinationVote
    {
        public GroupMemberDestinationVoteRepository(AppDbContext context) : base(context) { }
    }
}
