using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetDefaultUserRoleAsync();
}