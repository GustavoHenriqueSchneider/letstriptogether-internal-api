using WebApi.Models;
using WebApi.Repositories.Implementations;

namespace WebApi.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetByIdWithPreferencesAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserWithRelationshipsByIdAsync(Guid id);
    Task<User?> GetUserWithRolesByIdAsync(Guid id);
    Task<User?> GetUserWithRolesByEmailAsync(string email);
}
