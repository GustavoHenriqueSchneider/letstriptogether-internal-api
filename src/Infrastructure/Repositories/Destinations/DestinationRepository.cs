using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Destinations;

public class DestinationRepository : BaseRepository<Destination>, IDestinationRepository
{
    public DestinationRepository(AppDbContext context) : base(context) { }

    public new async Task<(IEnumerable<Destination> data, int hits)> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(d => d.Attractions)
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet.CountAsync();

        return (data, hits);
    }

    public new async Task<Destination?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(d => d.Attractions)
            .SingleOrDefaultAsync(e => e.Id == id);
    }
    
    public async Task<(IEnumerable<Destination> data, int hits)> GetNotVotedByUserInGroupAsync(
        Guid userId, Guid groupId, IEnumerable<string> groupPreferences, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(x => x.Attractions)
            .Where(x => !x.GroupMemberVotes.Any(v => 
                            v.GroupMember.GroupId == groupId && v.GroupMember.UserId == userId))
            .Where(x => x.Attractions.Any(d => groupPreferences.Contains(d.Category)))
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .AsNoTracking()
            .Where(x => !x.GroupMemberVotes.Any(v => 
                v.GroupMember.GroupId == groupId && v.GroupMember.UserId == userId))
            .Where(x => x.Attractions.Any(d => groupPreferences.Contains(d.Category)))
            .CountAsync();

        return (data, hits);
    }
}
