using Microsoft.EntityFrameworkCore;
using WebApi.Context;


namespace WebApi.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _dbSet.AsNoTracking()//para não rastrear as entidades, melhora performance
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToListAsync();//
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking()
                           .FirstOrDefaultAsync(e => EF
                           .Property<Guid>(e, "Id") == id);
    }
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}
