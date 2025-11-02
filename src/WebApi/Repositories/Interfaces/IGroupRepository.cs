using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IGroupRepository : IBaseRepository<Group>
{
    Task<Group?> GetGroupWithPreferencesAsync(Guid groupId);
    Task<Group?> GetGroupWithMembersAsync(Guid groupId);
    Task<Group?> GetGroupWithMembersPreferencesAsync(Guid groupId);
    Task<Group?> GetGroupWithMatchesAsync(Guid groupId);
    Task<(IEnumerable<Group> data, int hits)> GetAllGroupsByUserIdAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10);
    Task<bool> IsGroupMemberByUserIdAsync(Guid groupId, Guid userId);
}