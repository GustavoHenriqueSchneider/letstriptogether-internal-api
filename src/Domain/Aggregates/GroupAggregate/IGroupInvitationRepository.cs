using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupInvitationRepository : IBaseRepository<GroupInvitation>
{
    Task<(IEnumerable<GroupInvitation> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id, CancellationToken cancellationToken);
    Task<GroupInvitation?> GetByGroupAndStatusAsync(Guid groupId, Enums.GroupInvitationStatus status, CancellationToken cancellationToken);
}
