using LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.DestinationAggregate;

public interface IDestinationRepository : IBaseRepository<Destination>
{
    Task<(IEnumerable<Destination> data, int hits)> GetNotVotedByUserInGroupAsync(Guid userId, Guid groupId, 
        IEnumerable<string> groupPreferences, int pageNumber = 1, int pageSize = 10);
}
