using Domain.Aggregates.UserAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.UserAggregate;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdWithPreferencesAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserWithRelationshipsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetUserWithGroupMembershipsAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetUserWithRolesByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetUserWithRolesByEmailAsync(string email, CancellationToken cancellationToken);
}
