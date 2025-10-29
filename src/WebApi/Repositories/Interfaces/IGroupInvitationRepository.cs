using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IGroupInvitationRepository : IBaseRepository<GroupInvitation>
{
    Task<GroupInvitation?> GetByIdWithAnsweredByAsync(Guid id);
    Task<GroupInvitation?> GetByGroupIdAsync(Guid groupId);
}
