namespace WebApi.Persistence.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
}
