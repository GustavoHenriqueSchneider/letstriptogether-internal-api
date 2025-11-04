using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(AppDbContext context) : base(context) { }

    public async Task<Group?> GetGroupWithPreferencesAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Preferences)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<Group?> GetGroupWithMembersAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Members)
                .ThenInclude(x => x.User)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }
    
    public async Task<Group?> GetGroupWithMembersAndMatchesAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Members)
            .ThenInclude(x => x.User)
            .Include(x => x.Matches)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }
    
    public async Task<Group?> GetGroupWithMembersVotesAndMatchesAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Members)
                .ThenInclude(x => x.Votes)
            .Include(x => x.Matches)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<Group?> GetGroupWithMembersPreferencesAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Preferences)
            .Include(g => g.Members)
                .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Preferences)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<Group?> GetGroupWithMatchesAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(g => g.Matches)
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<(IEnumerable<Group> data, int hits)> GetAllGroupsByUserIdAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(g => g.Members.Any(m => m.UserId == userId))
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .Where(g => g.Members.Any(m => m.UserId == userId))
            .CountAsync();

        return (data, hits);
    }

    public async Task<bool> IsGroupMemberByUserIdAsync(Guid groupId, Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(g => g.Id == groupId && g.Members.Any(m => m.UserId == userId));
    }
}
