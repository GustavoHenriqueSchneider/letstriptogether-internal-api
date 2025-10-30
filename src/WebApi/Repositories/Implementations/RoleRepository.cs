using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Repositories.Implementations;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(AppDbContext context) : base(context) { }

    public async Task<Role?> GetDefaultUserRoleAsync()
    { 
        return await _dbSet.SingleOrDefaultAsync(r => r.Name == Roles.User);
    }
}