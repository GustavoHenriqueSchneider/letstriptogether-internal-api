using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;

public class UserRoleRepository
    : BaseRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(AppDbContext context) : base(context) { }
}
