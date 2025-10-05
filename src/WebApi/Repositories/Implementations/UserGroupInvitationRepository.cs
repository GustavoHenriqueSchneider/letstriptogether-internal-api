using System.Security.Cryptography.X509Certificates;
using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;
public class UserGroupInvitationRepository : BaseRepository<UserGroupInvitation>, IUserGroupInvitationRepository
{
    public UserGroupInvitationRepository(AppDbContext _dbSet) : base(_dbSet) { }
}
