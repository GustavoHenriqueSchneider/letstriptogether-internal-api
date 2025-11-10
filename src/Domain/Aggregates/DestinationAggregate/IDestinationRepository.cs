using Domain.Aggregates.DestinationAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.DestinationAggregate;

public interface IDestinationRepository : IBaseRepository<Destination>
{
    Task<(IEnumerable<Destination> data, int hits)> GetNotVotedByUserInGroupAsync(
        Guid userId, Guid groupId, IEnumerable<string> groupPreferences, 
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
}
