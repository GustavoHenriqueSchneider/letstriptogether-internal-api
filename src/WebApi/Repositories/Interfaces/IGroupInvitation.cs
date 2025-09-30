using WebApi.Models;

namespace WebApi.Repositories.Interfaces
{
    public interface IGroupInvitation : IBaseRepository<GroupInvitation>
    {
        Task<GroupInvitation?> GetByTokenAsync(string token);
    }
}
