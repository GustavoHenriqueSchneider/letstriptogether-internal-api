using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Groups;

public class GroupMemberRepository : BaseRepository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<GroupMember> data, int hits)> GetAllByGroupIdAsync(
        Guid groupId, int pageNumber = 1, int pageSize = 10)
    {
        var data = await _dbSet
            .AsNoTracking()
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var hits = await _dbSet.CountAsync(x => x.GroupId == groupId);

        return (data, hits);
    }

    public async Task<IEnumerable<GroupMember>> GetAllByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
}

