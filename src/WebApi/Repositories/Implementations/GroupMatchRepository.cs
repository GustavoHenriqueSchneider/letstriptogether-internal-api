using Microsoft.EntityFrameworkCore;
using WebApi.Repositories.Interfaces;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Implementations;

public class GroupMatchRepository : BaseRepository<GroupMatch>, IGroupMatchRepository
{
    public GroupMatchRepository(AppDbContext context) : base(context) { }

    public async Task<GroupMatch?>
        GetByIdWithRelationsAsync(Guid groupId, Guid id)
    {
        return await _dbSet
            .Include(x => x.Group)
            .Where(x => x.GroupId == groupId)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(IEnumerable<GroupMatch> data, int hits)>
        GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .AsNoTracking()
            .CountAsync(x => x.GroupId == groupId);

        return (data, hits);
    }
}
