using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Persistence.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : TrackableEntity
{
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _dbSet = context.Set<T>();
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<T> data, int hits)> GetAllAsync(
        int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var data = await _dbSet
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var hits = await _dbSet.CountAsync(cancellationToken);

        return (data, hits);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<int> GetHitsAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entityList, CancellationToken cancellationToken)
    {
        await _dbSet.AddRangeAsync(entityList, cancellationToken);
    }

    public async Task AddOrUpdateAsync(T entity, CancellationToken cancellationToken)
    {
        if (!await ExistsByIdAsync(entity.Id, cancellationToken))
        {
            await AddAsync(entity, cancellationToken);
            return;
        }

        Update(entity);
    }

    public void Update(T entity)
    {
        // TODO: fazer atualizar o status das entidades filhas se houver modificação
        entity.SetUpdateAt();
        
        var entry = _dbSet.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            try
            {
                _dbSet.Attach(entity);
            }
            catch (InvalidOperationException)
            {
                var trackedEntity = _dbSet.Find(entity.Id);
                if (trackedEntity is null)
                {
                    throw;
                }
                
                var trackedEntry = _dbSet.Entry(trackedEntity);
                
                trackedEntry.CurrentValues.SetValues(entity);
                trackedEntry.State = EntityState.Modified;
                
                return;
            }
        }
        
        entry.State = EntityState.Modified;
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
