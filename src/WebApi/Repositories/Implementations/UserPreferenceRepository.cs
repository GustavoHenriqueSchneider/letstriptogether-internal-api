using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserPreferenceRepository : BaseRepository<UserPreference>, IUserPreference
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }
}
