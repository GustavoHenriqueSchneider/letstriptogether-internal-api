using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.Clients;

public class RedisClient : IRedisClient
{
    public IDatabase Database { get; private set; }

    public RedisClient(string connectionString, ILogger<RedisClient> logger)
    {
        try
        {
            var connection = ConnectionMultiplexer.Connect(connectionString);
            Database = connection.GetDatabase();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao se conectar ao Redis");
            throw;
        }
    }
}
