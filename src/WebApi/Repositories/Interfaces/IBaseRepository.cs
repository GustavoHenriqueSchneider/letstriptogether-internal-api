using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IBaseRepository<T> where T : TrackableEntity
{
    Task<bool> ExistsByIdAsync(Guid id);
    Task<(IEnumerable<T> data, int hits)> GetAllAsync(int pageNumber = 1, int pageSize = 10);
    Task<T?> GetByIdAsync(Guid id);
    Task<int> GetHitsAsync();
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entityList);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entityList);
}
