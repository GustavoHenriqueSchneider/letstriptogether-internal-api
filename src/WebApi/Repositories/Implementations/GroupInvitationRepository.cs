using WebApi.Repositories.Interfaces;
using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;

namespace WebApi.Repositories.Implementations;

public class GroupInvitationRepository : BaseRepository<GroupInvitation>, IGroupInvitationRepository
{
    public GroupInvitationRepository(AppDbContext context) : base(context) { }
}
