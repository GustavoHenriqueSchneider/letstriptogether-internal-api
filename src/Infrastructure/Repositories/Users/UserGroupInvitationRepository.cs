using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Users;

public class UserGroupInvitationRepository 
    : BaseRepository<UserGroupInvitation>, IUserGroupInvitationRepository
{
    public UserGroupInvitationRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(ugi => ugi.UserId == userId && ugi.GroupInvitationId == groupInvitationId, cancellationToken);
    }
}
