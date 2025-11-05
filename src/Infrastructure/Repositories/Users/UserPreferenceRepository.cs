using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories.Users;

public class UserPreferenceRepository 
    : BaseRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }
}
