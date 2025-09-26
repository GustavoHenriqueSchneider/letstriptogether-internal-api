using WebApi.Models;

namespace WebApi.Repositories;

public class UserGroupInvitationRepository : BaseRepository<UserGroupInvitation>, IUserGroupInvitation
{
    public UserGroupInvitationRepository() : base() { }
}
