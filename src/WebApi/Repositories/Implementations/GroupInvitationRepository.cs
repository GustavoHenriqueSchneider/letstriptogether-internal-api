using WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Context.Implementations;

namespace WebApi.Repositories.Implementations;

namespace WebApi.Repositories.Implementations
{
public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitationRepository
{
        public GroupInvitationRepository(AppDbContext _dbSet) : base(_dbSet) { }
        
        public async Task<GroupInvitation?> GetByTokenAsync(string token)
        {
            return await _dbSet.AsNoTracking()
                                .Include(i => i.Group)
                                .FirstOrDefaultAsync(i => i.Token == token);
        }
    }
}
