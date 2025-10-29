using Microsoft.EntityFrameworkCore;
using WebApi.Repositories.Interfaces;
using WebApi.Models;
using WebApi.Context.Implementations;

namespace WebApi.Repositories.Implementations;

public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitationRepository
{
    public GroupInvitationRepository(AppDbContext context) : base(context) { }

    public async Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(gi => gi.AnsweredBy)
            .SingleOrDefaultAsync(gi => gi.Id == id);
    }

    public async Task<GroupInvitation?> GetByGroupIdAsync(Guid groupId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(gi => gi.AnsweredBy)
            .SingleOrDefaultAsync(gi => gi.GroupId == groupId);
    }
}
