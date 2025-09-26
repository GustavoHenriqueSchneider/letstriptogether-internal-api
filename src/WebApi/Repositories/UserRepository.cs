using WebApi.Context;
using WebApi.Models;


namespace WebApi.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository (AppDbContext context) : base(context) {}
}
