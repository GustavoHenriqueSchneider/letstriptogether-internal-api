using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserRoleRepository
    : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }
}
