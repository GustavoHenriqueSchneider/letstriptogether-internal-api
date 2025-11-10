using Domain.Aggregates.UserAggregate;
using Domain.Aggregates.UserAggregate.Entities;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.Repositories.Users;

public class UserPreferenceRepository 
    : BaseRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }
}
