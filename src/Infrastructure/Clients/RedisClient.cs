using StackExchange.Redis;

namespace LetsTripTogether.InternalApi.Infrastructure.Clients;

public class RedisClient : IRedisClient
{
    public IDatabase Database { get; private set; }

    public RedisClient(string connectionString)
    {
        var connection = ConnectionMultiplexer.Connect(connectionString);
        Database = connection.GetDatabase();
    }
}
