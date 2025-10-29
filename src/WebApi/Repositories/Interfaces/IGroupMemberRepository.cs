using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupMemberRepository : IBaseRepository<GroupMember>
{
    Task<(IEnumerable<GroupMember> data, int hits)> GetAllByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
}
