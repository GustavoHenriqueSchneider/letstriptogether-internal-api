using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IUserGroupInvitationRepository : IBaseRepository<UserGroupInvitation>
{
    Task<bool> ExistsByUserIdAndGroupInvitationIdAsync(Guid userId, Guid groupInvitationId);
}
