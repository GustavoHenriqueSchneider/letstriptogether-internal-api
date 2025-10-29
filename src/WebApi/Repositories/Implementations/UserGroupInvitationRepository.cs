using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserGroupInvitationRepository 
    : BaseRepository<UserGroupInvitation>, IUserGroupInvitationRepository
{
    public UserGroupInvitationRepository(AppDbContext context) : base(context) { }

    public async Task<UserGroupInvitation?> GetByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId)
    {
        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(ugi => ugi.UserId == userId && ugi.GroupInvitationId == groupInvitationId);
    }
}
