using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(AppDbContext context) : base(context) { }

    public async Task<GroupMember?> GetByIdWithRelationsAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.User)
            .Include(x => x.Group)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(IEnumerable<GroupMember> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .CountAsync(x => x.GroupId == groupId);

        return (data, hits);
    }

    public async Task<bool> ExistsByGroupAndUserAsync(Guid groupId, Guid userId)
    {
        return await _dbSet
            .AnyAsync(x => x.GroupId == groupId && x.UserId == userId);
    }
}

