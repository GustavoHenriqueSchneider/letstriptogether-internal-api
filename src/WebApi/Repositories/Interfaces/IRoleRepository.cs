using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Interfaces;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetDefaultUserRoleAsync();
}