using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(AppDbContext context) : base(context) { }
    public async Task<GroupMember?> FindMemberAsync(Guid groupId, Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
    }
    public async Task<GroupMember?> GetByGroupIdAsync(Guid groupId)
    {
        return await _dbSet.FirstOrDefaultAsync(gm => gm.GroupId == groupId);
    }
}

