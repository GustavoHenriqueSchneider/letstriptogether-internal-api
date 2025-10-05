using WebApi.Models;

namespace WebApi.Repositories.Interfaces;
public interface IGroupInvitationRepository : IBaseRepository<GroupInvitation>
{
    Task<GroupInvitation?> GetByTokenAsync(string token);
}
