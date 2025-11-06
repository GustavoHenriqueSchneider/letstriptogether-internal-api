using LetsTripTogether.InternalApi.Domain.Common;
using LetsTripTogether.InternalApi.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace LetsTripTogether.InternalApi.Infrastructure.Repositories;

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
        entity.SetUpdateAt();
        
        var entry = _dbSet.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            var trackedEntity = _dbSet.Find(entity.Id);
            if (trackedEntity != null)
            {
                var trackedEntry = _dbSet.Entry(trackedEntity);
                trackedEntry.CurrentValues.SetValues(entity);
                trackedEntry.State = EntityState.Modified;
                return;
            }
            
            try
            {
                _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
            catch (InvalidOperationException)
            {
                // Entity might be tracked in a different way, try to update by ID
                var existingEntity = _dbSet.Local.FirstOrDefault(e => e.Id == entity.Id);
                if (existingEntity != null)
                {
                    var existingEntry = _dbSet.Entry(existingEntity);
                    existingEntry.CurrentValues.SetValues(entity);
                    existingEntry.State = EntityState.Modified;
                    return;
                }
                
                throw;
            }
        }
        else
        {
            entry.State = EntityState.Modified;
        }
    }

    public void Remove(T entity)
    {
        var entry = _dbSet.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            // Try to find tracked entity by Id (works for single key entities)
            // For composite key entities, we need to check if already tracked differently
            try
            {
                var trackedEntity = _dbSet.Find(entity.Id);
                if (trackedEntity != null)
                {
                    _dbSet.Remove(trackedEntity);
                    return;
                }
            }
            catch (ArgumentException)
            {
                // Entity has composite key, Find() won't work with just Id
                // Check if entity is already tracked by comparing with existing tracked entities
                var trackedEntities = _dbSet.Local.Where(e => e.Id == entity.Id).ToList();
                if (trackedEntities.Any())
                {
                    foreach (var tracked in trackedEntities)
                    {
                        _dbSet.Remove(tracked);
                    }
                    return;
                }
            }
        }
        
        _dbSet.Remove(entity);
    }
    
    public void RemoveRange(IEnumerable<T> entityList)
    {
        entityList.ToList().ForEach(Remove);
    }
}
