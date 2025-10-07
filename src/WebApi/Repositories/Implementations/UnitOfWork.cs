using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; private set; }
    public IBaseRepository<Destination> Destinations { get; private set; }
    public IBaseRepository<Group> Groups { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        this.Users = new UserRepository(context);
        this.Destinations = new BaseRepository<Destination>(context);
        this.Groups = new BaseRepository<Group>(context);
    }
    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
