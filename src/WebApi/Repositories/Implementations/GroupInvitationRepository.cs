using WebApi.Repositories.Interfaces;
using WebApi.Models;
using WebApi.Context.Implementations;

namespace WebApi.Repositories.Implementations;

public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitationRepository
{
    public GroupInvitationRepository(AppDbContext _dbSet) : base(_dbSet) { }
}
