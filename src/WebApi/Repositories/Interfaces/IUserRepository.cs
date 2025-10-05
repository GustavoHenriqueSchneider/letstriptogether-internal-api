using WebApi.Models;
using WebApi.Repositories.Implementations;

namespace WebApi.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email);

    Task<User?> GetByIdWithPreferencesAsync(Guid id);

    Task<User?> GetByEmailAsync( string email);

    Task<Role?> GetDefaultUserRoleAsync();
    
}
