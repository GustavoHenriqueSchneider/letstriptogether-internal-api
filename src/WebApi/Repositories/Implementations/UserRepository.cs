using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;


namespace WebApi.Repositories.Implementations;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository (AppDbContext context) : base(context) {}
}
