using LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.GroupAggregate;

public interface IGroupRepository : IBaseRepository<Group>
{
    Task<Group?> GetGroupWithPreferencesAsync(Guid groupId);
    Task<Group?> GetGroupWithMembersAsync(Guid groupId);
    Task<Group?> GetGroupWithMembersAndMatchesAsync(Guid groupId);
    Task<Group?> GetGroupWithMembersVotesAndMatchesAsync(Guid groupId);
    Task<Group?> GetGroupWithMembersPreferencesAsync(Guid groupId);
    Task<Group?> GetGroupWithMatchesAsync(Guid groupId);
    Task<(IEnumerable<Group> data, int hits)> GetAllGroupsByUserIdAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<bool> IsGroupMemberByUserIdAsync(Guid groupId, Guid userId);
}