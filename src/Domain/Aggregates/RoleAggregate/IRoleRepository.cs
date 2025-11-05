using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken);
}