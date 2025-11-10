using Domain.Aggregates.RoleAggregate.Entities;
using Domain.Common;

namespace Domain.Aggregates.RoleAggregate;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken);
}