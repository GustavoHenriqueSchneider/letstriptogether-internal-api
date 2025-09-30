using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations
{
    public class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMember
    {
        public GroupMemberRepository(AppDbContext context) : base(context) { }
    }
}
