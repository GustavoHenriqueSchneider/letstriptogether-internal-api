using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.RoleAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using RoleType = LetsTripTogether.InternalApi.Domain.Security.Role;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Roles;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetDefaultUserRoleAsync()
    { 
        return await _dbSet.SingleOrDefaultAsync(r => r.Name == RoleType.User);
    }
}