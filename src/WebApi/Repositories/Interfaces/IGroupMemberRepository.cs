using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IGroupMemberRepository : IBaseRepository<GroupMember>
{
    Task<(IEnumerable<GroupMember> data, int hits)> GetAllByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<IEnumerable<GroupMember>> GetAllByUserIdAsync(Guid userId);
}
