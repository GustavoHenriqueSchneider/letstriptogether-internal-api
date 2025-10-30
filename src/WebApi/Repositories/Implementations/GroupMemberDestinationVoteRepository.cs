using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupMemberDestinationVoteRepository 
    : BaseRepository<GroupMemberDestinationVote> , IGroupMemberDestinationVoteRepository
{
    public GroupMemberDestinationVoteRepository(AppDbContext context) : base(context) { }

    public async Task<GroupMemberDestinationVote?>
        GetByIdWithRelationsAsync(Guid groupId, Guid id)
    {
        return await _dbSet
            .Include(x => x.GroupMember)
                .ThenInclude(x => x.Group)
            .Where(x => x.GroupMember.GroupId == groupId)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)>
        GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupMember.GroupId == groupId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .AsNoTracking()
            .CountAsync(x => x.GroupMember.GroupId == groupId);

        return (data, hits);
    }

    public async Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)>
        GetByMemberIdAsync(Guid memberId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupMember.Id == memberId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet
            .AsNoTracking()
            .CountAsync(x => x.GroupMember.Id == memberId);

        return (data, hits);
    }

    public async Task<bool> ExistsByGroupMemberDestinationVoteByIdsAsync(Guid memberId, Guid destinationId)
    {
        return await _dbSet
            .AnyAsync(x => x.GroupMemberId == memberId && x.DestinationId == destinationId);
    }
}
