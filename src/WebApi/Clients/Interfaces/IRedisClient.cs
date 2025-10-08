using StackExchange.Redis;

namespace WebApi.Clients.Interfaces;

public interface IRedisClient
{
    IDatabase Database { get; }
}
