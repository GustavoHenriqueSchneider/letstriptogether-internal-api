using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupMemberDestinationVoteRepository : IBaseRepository<GroupMemberDestinationVote>
{
    Task<GroupMemberDestinationVote?> GetByIdWithRelationsAsync(Guid groupId, Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)> GetByMemberIdAsync(Guid memberId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<bool> ExistsByGroupMemberDestinationVoteByIdsAsync(Guid memberId, Guid destinationId, CancellationToken cancellationToken);
    Task<GroupMemberDestinationVote?> GetByMemberAndDestinationAsync(Guid memberId, Guid destinationId, CancellationToken cancellationToken);
}
