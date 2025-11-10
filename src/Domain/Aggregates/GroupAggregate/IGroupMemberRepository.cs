using Domain.Aggregates.GroupAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.GroupAggregate;

public interface IGroupMemberRepository : IBaseRepository<GroupMember>
{
    Task<(IEnumerable<GroupMember> data, int hits)> GetAllByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<GroupMember>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
