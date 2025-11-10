using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.GroupAggregate;

public interface IGroupPreferenceRepository : IBaseRepository<GroupPreference>
{
    Task<GroupPreference?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
}
