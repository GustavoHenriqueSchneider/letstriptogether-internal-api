using StackExchange.Redis;
using WebApi.Clients.Interfaces;

namespace WebApi.Clients.Implementations;

public class RedisClient : IRedisClient
{
    private readonly ConnectionMultiplexer _connection;
    public IDatabase Database { get; private set; }

    public RedisClient(string connectionString)
    {
        _connection = ConnectionMultiplexer.Connect(connectionString);
        Database = _connection.GetDatabase();
    }
}
