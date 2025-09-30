using WebApi.Models;

namespace WebApi.Repositories.Interfaces;
public interface IBaseRepository<T> where T : TrackableEntity
{
    Task<IEnumerable<T>> GetAllAsync( int pageNumber, int pageSize);
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}
