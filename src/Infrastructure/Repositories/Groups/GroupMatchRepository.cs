using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Groups;

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

    public async Task<GroupMatch?> GetByGroupAndDestinationAsync(Guid groupId, Guid destinationId)
    {
        return await _dbSet
            .SingleOrDefaultAsync(x => x.GroupId == groupId && x.DestinationId == destinationId);
    }

    public async Task<IEnumerable<GroupMatch>> GetAllMatchesByGroupAsync(Guid groupId)
    {
        return await _dbSet
            .Where(x => x.GroupId == groupId)
            .ToListAsync();
    }
}
