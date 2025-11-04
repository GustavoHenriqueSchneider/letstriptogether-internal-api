using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Enums;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupInvitationRepository : IBaseRepository<GroupInvitation>
{
    Task<(IEnumerable<GroupInvitation> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id);
    Task<GroupInvitation?> GetByGroupAndStatusAsync(Guid groupId, GroupInvitationStatus status);
}
