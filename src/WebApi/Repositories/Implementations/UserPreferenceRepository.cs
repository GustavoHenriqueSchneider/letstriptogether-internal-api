using WebApi.Context.Implementations;
using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UserPreferenceRepository 
    : BaseRepository<UserPreference>, IUserPreferenceRepository
{
    public UserPreferenceRepository(AppDbContext context) : base(context) { }
}
