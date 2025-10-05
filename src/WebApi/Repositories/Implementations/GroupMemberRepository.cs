using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations
{
    public class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMemberRepository
    {
        public GroupMemberRepository(AppDbContext _dbSet) : base(_dbSet) { }
    }
}
