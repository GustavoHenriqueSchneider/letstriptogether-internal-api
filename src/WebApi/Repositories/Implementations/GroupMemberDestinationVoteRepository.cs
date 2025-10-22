using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupMemberDestinationVoteRepository 
    : BaseRepository<GroupMemberDestinationVote> , IGroupMemberDestinationVoteRepository
{
    public GroupMemberDestinationVoteRepository(AppDbContext context) : base(context) { }

    public async Task<GroupMemberDestinationVote?> GetByIdWithRelationsAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.GroupMember)
                .ThenInclude(x => x.User)
            .Include(x => x.GroupMember)
                .ThenInclude(x => x.Group)
            .Include(x => x.Destination)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)> GetAllWithRelationsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(x => x.GroupMember)
                .ThenInclude(x => x.User)
            .Include(x => x.Destination)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet.CountAsync();

        return (data, hits);
    }

    public async Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Include(x => x.GroupMember)
                .ThenInclude(x => x.User)
            .Include(x => x.Destination)
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
}
