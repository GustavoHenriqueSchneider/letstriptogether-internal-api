using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupPreferenceRepository : IBaseRepository<GroupPreference>
{
    Task<GroupPreference?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
}
