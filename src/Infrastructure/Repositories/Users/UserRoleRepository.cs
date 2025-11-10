using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.Repositories.Users;

public class UserRoleRepository
    : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }
}
