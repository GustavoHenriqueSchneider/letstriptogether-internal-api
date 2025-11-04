using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetByIdWithPreferencesAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserWithRelationshipsByIdAsync(Guid id);
    Task<User?> GetUserWithGroupMembershipsAsync(Guid id);
    Task<User?> GetUserWithRolesByIdAsync(Guid id);
    Task<User?> GetUserWithRolesByEmailAsync(string email);
}
