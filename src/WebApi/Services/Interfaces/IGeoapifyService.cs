using WebApi.Models;

namespace WebApi.Services.Interfaces;

public interface IGeoapifyService
{
    Task<List<Destination>> GetNewDestinationsAsync(int pageSize = 10);
}
