using StackExchange.Redis;

namespace Infrastructure.Clients;

public interface IRedisClient
{
    IDatabase Database { get; }
}
