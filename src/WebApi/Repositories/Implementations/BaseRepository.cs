using Microsoft.EntityFrameworkCore;
using WebApi.Context.Implementations;
using WebApi.Models;
using WebApi.Repositories.Interfaces;

namespace WebApi.Repositories.Implementations;

public class BaseRepository<T> : IBaseRepository<T> where T : TrackableEntity
{
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _dbSet = context.Set<T>();
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

    public async Task AddRangeAsync(IEnumerable<T> entityList)
    {
        await _dbSet.AddRangeAsync(entityList);
    }

    public void Update(T entity)
    {
        // TODO: fazer atualizar o status das entidades filhas se houver modificação
        _dbSet.Attach(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entityList)
    {
        _dbSet.RemoveRange(entityList);
    }
}
