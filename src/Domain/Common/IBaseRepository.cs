namespace LetsTripTogether.InternalApi.Domain.Common;

public interface IBaseRepository<T> where T : TrackableEntity
{
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(IEnumerable<T> data, int hits)> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<int> GetHitsAsync(CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<T> entityList, CancellationToken cancellationToken);
    Task AddOrUpdateAsync(T entity, CancellationToken cancellationToken);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entityList);
}
