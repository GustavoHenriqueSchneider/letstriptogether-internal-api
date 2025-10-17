using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(AppDbContext context) : base(context) { }

    public async Task<Group?> GetGroupWithMembersAsync(Guid groupId)
    {
        return await _dbSet
            .Include(g => g.Members)
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == groupId);
    }
}
