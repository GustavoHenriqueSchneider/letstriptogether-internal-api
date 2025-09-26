using WebApi.Models;

namespace WebApi.Repositories;

public class UserPreferenceRepository : BaseRepository<UserPreference>, IUserPreference
{
    public UserPreferenceRepository() : base() { }
}
