using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupMemberDestinationVoteRepository : IBaseRepository<GroupMemberDestinationVote>
{
    Task<GroupMemberDestinationVote?> GetByIdWithRelationsAsync(Guid groupId, Guid id);
    Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<(IEnumerable<GroupMemberDestinationVote> data, int hits)> GetByMemberIdAsync(Guid memberId, int pageNumber = 1, int pageSize = 10);
    Task<bool> ExistsByGroupMemberDestinationVoteByIdsAsync(Guid memberId, Guid destinationId);
    Task<GroupMemberDestinationVote?> GetByMemberAndDestinationAsync(Guid memberId, Guid destinationId);
}
