using WebApi.Context.Implementations;
using WebApi.Persistence.Interfaces;

namespace WebApi.Persistence.Implementations;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
