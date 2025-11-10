using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.GroupAggregate;

public interface IGroupMatchRepository : IBaseRepository<GroupMatch>
{
    Task<GroupMatch?> GetByIdWithRelationsAsync(Guid groupId, Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<GroupMatch> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<GroupMatch?> GetByGroupAndDestinationAsync(Guid groupId, Guid destinationId, CancellationToken cancellationToken);
    Task<IEnumerable<GroupMatch>> GetAllMatchesByGroupAsync(Guid groupId, CancellationToken cancellationToken);
}
