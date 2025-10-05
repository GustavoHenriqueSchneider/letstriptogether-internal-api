using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;
using WebApi.Security;


namespace WebApi.Repositories.Implementations;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) : base(context) 
    {
    _context = context;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AsNoTracking().AnyAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdWithPreferencesAsync(Guid id)
    {
        return await _dbSet
            .Include(x => x.Preferences) 
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Role?> GetDefaultUserRoleAsync()
    {
        return await _context.Roles.SingleOrDefaultAsync(x => x.Name == Roles.User);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email);
    }
}