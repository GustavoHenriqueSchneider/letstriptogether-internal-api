using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupMemberRepository : IBaseRepository<GroupMember>
{
    Task <GroupMember?> GetByGroupIdAsync(Guid groupId);
    Task<GroupMember?> FindMemberAsync(Guid groupId, Guid userId);
}
