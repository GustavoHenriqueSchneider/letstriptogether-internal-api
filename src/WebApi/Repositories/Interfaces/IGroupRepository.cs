using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupRepository : IBaseRepository<Group>
{
    Task<Group?> GetGroupWithMembersAsync(Guid groupId);
}