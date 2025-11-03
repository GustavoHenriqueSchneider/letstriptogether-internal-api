using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

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
        Guid userId, Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x =>
                !x.GroupMemberVotes.Any(v => 
                    v.GroupMember.GroupId == groupId && v.GroupMember.UserId == userId))
            .OrderByDescending(d => d.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .AsNoTracking()
            .Where(x =>
                !x.GroupMemberVotes.Any(v => 
                    v.GroupMember.GroupId == groupId && v.GroupMember.UserId == userId))
            .CountAsync();

        return (data, hits);
    }
}
