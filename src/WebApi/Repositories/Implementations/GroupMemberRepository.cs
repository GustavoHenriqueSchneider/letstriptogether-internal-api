using WebApi.Context.Implementations;
using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(AppDbContext context) : base(context) { }
}
