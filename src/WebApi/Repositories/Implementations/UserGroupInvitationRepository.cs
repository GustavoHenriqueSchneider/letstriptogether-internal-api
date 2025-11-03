using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserGroupInvitationRepository 
    : BaseRepository<UserGroupInvitation>, IUserGroupInvitationRepository
{
    public UserGroupInvitationRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(ugi => ugi.UserId == userId && ugi.GroupInvitationId == groupInvitationId);
    }
}
