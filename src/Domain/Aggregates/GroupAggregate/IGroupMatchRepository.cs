using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupMatchRepository : IBaseRepository<GroupMatch>
{
    Task<GroupMatch?> GetByIdWithRelationsAsync(Guid groupId, Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<GroupMatch> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<GroupMatch?> GetByGroupAndDestinationAsync(Guid groupId, Guid destinationId, CancellationToken cancellationToken);
    Task<IEnumerable<GroupMatch>> GetAllMatchesByGroupAsync(Guid groupId, CancellationToken cancellationToken);
}
