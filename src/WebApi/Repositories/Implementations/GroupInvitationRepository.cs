using WebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Context;


namespace WebApi.Repositories.Implementations
{
    public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitation
    {
        public GroupInvitationRepository(AppDbContext context) : base(context) { }
        
        public async Task<GroupInvitation?> GetByTokenAsync(string token)
        {
            return await _dbSet.AsNoTracking()
                                .Include(i => i.Group)
                                .FirstOrDefaultAsync(i => i.Token == token);
        }
    }
}
