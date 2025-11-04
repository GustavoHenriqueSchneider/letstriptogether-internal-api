using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;

public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitationRepository
{
    public GroupInvitationRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<GroupInvitation> data, int hits)> GetByGroupIdAsync(
        Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var hits = await _dbSet
            .AsNoTracking()
            .CountAsync(x => x.GroupId == groupId, cancellationToken);

        return (data, hits);
    }

    public async Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(gi => gi.AnsweredBy)
            .SingleOrDefaultAsync(gi => gi.Id == id, cancellationToken);
    }

    public async Task<GroupInvitation?> GetByGroupAndStatusAsync(Guid groupId, GroupInvitationStatus status, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(gi => gi.AnsweredBy)
            .SingleOrDefaultAsync(gi => gi.GroupId == groupId && gi.Status == status, cancellationToken);
    }
}
