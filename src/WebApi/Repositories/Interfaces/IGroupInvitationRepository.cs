using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IGroupInvitationRepository : IBaseRepository<GroupInvitation>
{
    Task<(IEnumerable<GroupInvitation> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id);
    Task<GroupInvitation?> GetByGroupIdAsync(Guid groupId);
}
