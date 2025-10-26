using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupRepository : IBaseRepository<Group>
{
    Task<Group?> GetGroupWithMembersAsync(Guid groupId);
    Task<(IEnumerable<Group> data, int hits)> GetAllGroupsByUserIdAsync(
        Guid userId, int pageNumber = 1, int pageSize = 10);
}