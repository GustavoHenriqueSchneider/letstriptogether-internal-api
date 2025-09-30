using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations
{
    public class GroupRepository : BaseRepository<Group>, IGroup
    {
        public GroupRepository(AppDbContext context) : base(context) { }
    }
}
