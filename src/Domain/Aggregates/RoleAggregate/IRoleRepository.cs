using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Domain.Common;

namespace LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;

public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken);
}