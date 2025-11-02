using Microsoft.EntityFrameworkCore;
using WebApi.Repositories.Interfaces;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Implementations;

public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitationRepository
{
    public GroupInvitationRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<GroupInvitation> data, int hits)> GetByGroupIdAsync(
        Guid groupId, int pageNumber = 1, int pageSize = 10)
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
