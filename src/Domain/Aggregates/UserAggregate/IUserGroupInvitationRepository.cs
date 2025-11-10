using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.UserAggregate;

public interface IUserGroupInvitationRepository : IBaseRepository<UserGroupInvitation>
{
    Task<bool> ExistsByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId, CancellationToken cancellationToken);
}
