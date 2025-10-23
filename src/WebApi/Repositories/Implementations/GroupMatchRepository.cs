using Microsoft.EntityFrameworkCore;
using WebApi.Repositories.Interfaces;
using WebApi.Models;
using WebApi.Context.Implementations;

namespace WebApi.Repositories.Implementations;

public class GroupMatchRepository : BaseRepository<GroupMatch>, IGroupMatchRepository
{
    public GroupMatchRepository(AppDbContext context) : base(context) { }

    public new async Task<(IEnumerable<GroupMatch> data, int hits)> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Destination)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet.CountAsync();

        return (data, hits);
    }

    public async Task<GroupMatch?> GetByIdWithRelationsAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Group)
            .Include(x => x.Destination)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(IEnumerable<GroupMatch> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(x => x.Group)
            .Include(x => x.Destination)
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
