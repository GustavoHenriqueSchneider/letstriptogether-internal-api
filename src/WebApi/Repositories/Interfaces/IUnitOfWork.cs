using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBaseRepository<User> Users { get; }
    IBaseRepository<Destination> Destinations { get; }
    Task<int> SaveAsync();
}
