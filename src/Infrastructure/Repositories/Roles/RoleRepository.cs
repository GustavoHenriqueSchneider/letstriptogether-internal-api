using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories.Roles;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetDefaultUserRoleAsync(CancellationToken cancellationToken)
    { 
        return await _dbSet.SingleOrDefaultAsync(r => r.Name == Domain.Security.Roles.User, cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        return await _dbSet.SingleOrDefaultAsync(x => x.Name.ToLower() == roleName.ToLower(), cancellationToken);
    }
}