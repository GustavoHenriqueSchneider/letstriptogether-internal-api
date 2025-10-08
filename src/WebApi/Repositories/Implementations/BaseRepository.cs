using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;
using WebApi.Repositories.Interfaces;


namespace WebApi.Repositories.Implementations;

public class BaseRepository<T> : IBaseRepository<T> where T : TrackableEntity
{
    private readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _dbSet.AsNoTracking()
                           .OrderByDescending(e => e.CreatedAt)
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToListAsync();
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id);
    }
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
    public void Update(T entity)
    {
        _dbSet.Attach(entity);
    }
    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}
