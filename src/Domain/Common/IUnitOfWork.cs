namespace LetsTripTogether.InternalApi.Domain.Common;

public interface IUnitOfWork : IDisposable
{
    Task SaveAsync(CancellationToken cancellationToken);
}
