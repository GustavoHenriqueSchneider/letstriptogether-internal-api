using WebApi.Context.Implementations;
using WebApi.Models.Aggregates;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserPreferenceRepository 
    : BaseRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }
}
