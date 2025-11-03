using WebApi.Models.Aggregates;
using WebApi.Models.Enums;

namespace WebApi.Repositories.Interfaces;

public interface IGroupInvitationRepository : IBaseRepository<GroupInvitation>
{
    Task<(IEnumerable<GroupInvitation> data, int hits)> GetByGroupIdAsync(Guid groupId, int pageNumber = 1, int pageSize = 10);
    Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id);
    Task<GroupInvitation?> GetByGroupAndStatusAsync(Guid groupId, GroupInvitationStatus status);
}
