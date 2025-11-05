using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupRepository : IBaseRepository<Group>
{
    Task<Group?> GetGroupWithPreferencesAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Group?> GetGroupWithMembersAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Group?> GetGroupWithMembersAndMatchesAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Group?> GetGroupWithMembersVotesAndMatchesAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Group?> GetGroupWithMembersPreferencesAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Group?> GetGroupWithMatchesAsync(Guid groupId, CancellationToken cancellationToken);
    Task<(IEnumerable<Group> data, int hits)> GetAllGroupsByUserIdAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<bool> IsGroupMemberByUserIdAsync(Guid groupId, Guid userId, CancellationToken cancellationToken);
}