using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;
using WebApi.Security;

namespace WebApi.Repositories.Implementations;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _dbSet.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdWithPreferencesAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.Preferences) 
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserWithRelationshipsByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(u => u.GroupMemberships)
            .Include(u => u.AcceptedInvitations)
            .Include(u => u.Preferences)
            .Include(u => u.UserRoles)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserWithRolesByIdAsync(Guid id)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> GetUserWithRolesByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .SingleOrDefaultAsync(x => x.Email == email);
    }
}