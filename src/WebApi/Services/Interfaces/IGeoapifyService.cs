using WebApi.Models.Aggregates;

namespace WebApi.Services.Interfaces;

public interface IGeoapifyService
{
    Task<List<Destination>> GetNewDestinationsAsync(int pageSize = 10);
}
