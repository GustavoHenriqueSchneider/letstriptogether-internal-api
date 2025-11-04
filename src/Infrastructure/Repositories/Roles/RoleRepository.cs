using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Roles;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken)
    { 
        return await _dbSet.SingleOrDefaultAsync(r => r.Name == Domain.Security.Roles.User, cancellationToken);
    }
}