using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IBaseRepository<User> Users { get; private set; }//ára evitar mudanças externas
    public IBaseRepository<Destination> Destinations { get; private set; }
    public IBaseRepository<Group> Groups { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        //Cria instancias dos repositórios
        Users = new BaseRepository<User>(_context);
        Destinations = new BaseRepository<Destination>(_context);
        Groups = new BaseRepository<Group>(_context);
    }
    public async Task<int> SaveAsync()//faz a persistencia
    {
        return await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        //libera recursos
        _context.Dispose();
    }
}
