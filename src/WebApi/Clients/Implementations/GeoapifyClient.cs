using WebApi.Clients.Interfaces;

namespace WebApi.Clients.Implementations;

public class GeoapifyClient(HttpClient httpClient) : IGeoapifyClient
{
    public HttpClient Client { get; private set; } = httpClient;
}
