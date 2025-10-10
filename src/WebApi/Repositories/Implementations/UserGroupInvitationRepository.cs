using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserGroupInvitationRepository 
    : BaseRepository<UserGroupInvitation>, IUserGroupInvitationRepository
{
    public UserGroupInvitationRepository(AppDbContext context) : base(context) { }
}
