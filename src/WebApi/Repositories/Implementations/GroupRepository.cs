using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(AppDbContext context) : base(context) { }

    public async Task<Group?> GetGroupWithMembersAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Members)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<(IEnumerable<Group> data, int hits)> GetAllGroupsByUserIdAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(g => g.Members)
            .ThenInclude(x => x.UserId == userId)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .Include(g => g.Members)
            .ThenInclude(x => x.UserId == userId)
            .CountAsync();

        return (data, hits);
    }
}
