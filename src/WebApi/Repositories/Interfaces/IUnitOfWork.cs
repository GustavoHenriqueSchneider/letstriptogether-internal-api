using WebApi.Models;

namespace WebApi.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
}
