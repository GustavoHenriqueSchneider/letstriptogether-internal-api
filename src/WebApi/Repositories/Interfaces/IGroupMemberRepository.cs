using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupMemberRepository : IBaseRepository<GroupMember>
{
    Task<GroupMember?> GetByIdWithRelationsAsync(Guid id);
    Task<(IEnumerable<GroupMember> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<bool> ExistsByGroupAndUserAsync(Guid groupId, Guid userId);
}
