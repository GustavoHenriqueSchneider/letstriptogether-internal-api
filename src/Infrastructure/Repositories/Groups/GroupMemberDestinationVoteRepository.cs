using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories.Groups;

public class GroupMemberDestinationVoteRepository 
    : BaseRepository<GroupMemberDestinationVote> , IGroupMemberDestinationVoteRepository
{
    public GroupMemberDestinationVoteRepository(AppDbContext context) : base(context) { }

    public async Task<GroupMemberDestinationVote?>
        GetByIdWithRelationsAsync(Guid groupId, Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .Include(x => x.GroupMember)
            .ThenInclude(x => x.Group)
            .Where(x => x.GroupMember.GroupId == groupId)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)>
        GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupMember.GroupId == groupId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var hits = await _dbSet
            .AsNoTracking()
            .CountAsync(x => x.GroupMember.GroupId == groupId, cancellationToken);

        return (data, hits);
    }

    public async Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)>
        GetByMemberIdAsync(Guid memberId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupMember.Id == memberId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var hits = await _dbSet
            .AsNoTracking()
            .CountAsync(x => x.GroupMember.Id == memberId, cancellationToken);

        return (data, hits);
    }

    public async Task<bool> ExistsByGroupMemberDestinationVoteByIdsAsync(Guid memberId, Guid destinationId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AnyAsync(x => x.GroupMemberId == memberId && x.DestinationId == destinationId, cancellationToken);
    }
    
    public async Task<GroupMemberDestinationVote?> GetByMemberAndDestinationAsync(Guid memberId, Guid destinationId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .SingleOrDefaultAsync(x => x.GroupMemberId == memberId 
                                       && x.DestinationId == destinationId, cancellationToken);
    }
}
