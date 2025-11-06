using StackExchange.Redis;

namespace LetsTripTogether.InternalApi.Infrastructure.Clients;

public interface IRedisClient
{
    IDatabase Database { get; }
}
