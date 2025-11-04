using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

public interface IUserGroupInvitationRepository : IBaseRepository<UserGroupInvitation>
{
    Task<bool> ExistsByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId, CancellationToken cancellationToken);
}
