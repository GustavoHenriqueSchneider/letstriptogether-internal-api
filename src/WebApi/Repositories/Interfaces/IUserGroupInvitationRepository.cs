using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IUserGroupInvitationRepository : IBaseRepository<UserGroupInvitation>
{
    Task<UserGroupInvitation?> GetByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId);
}
