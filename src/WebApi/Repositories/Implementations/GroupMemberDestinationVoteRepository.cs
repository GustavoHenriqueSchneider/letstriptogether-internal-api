using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations
{
    public class GroupMemberDestinationVoteRepository : BaseRepository<GroupMemberDestinationVote> , IGroupMemberDestinationVoteRepository
    {
        public GroupMemberDestinationVoteRepository(AppDbContext _dbSet) : base(_dbSet) { }
    }
}
