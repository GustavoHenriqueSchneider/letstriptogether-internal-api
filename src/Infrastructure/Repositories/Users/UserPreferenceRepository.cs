using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate;
using LetsTripTogether.InternalApi.Domain.Aggregates.UserAggregate.Entities;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories.Users;

public class UserPreferenceRepository 
    : BaseRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }
}
