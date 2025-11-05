using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

public interface IUserGroupInvitationRepository : IBaseRepository<UserGroupInvitation>
{
    Task<bool> ExistsByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId, CancellationToken cancellationToken);
}
