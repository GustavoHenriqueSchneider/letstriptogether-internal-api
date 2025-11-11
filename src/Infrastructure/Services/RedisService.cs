using Application.Common.Interfaces.Services;
using Infrastructure.Clients;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class RedisService(IRedisClient redisClient, ILogger<RedisService> logger) : IRedisService
{
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            return await redisClient.Database.StringGetAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao obter valor do Redis para chave {Key}", key);
            throw;
        }
    }

    public async Task SetAsync(string key, string value, int ttlSeconds)
    {
        try
        {
            await redisClient.Database.StringSetAsync(key, value, TimeSpan.FromSeconds(ttlSeconds));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao definir valor no Redis para chave {Key}", key);
            throw;
        }
    }

    public async Task DeleteAsync(string key)
    {
        try
        {
            await redisClient.Database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao deletar chave {Key} do Redis", key);
            throw;
        }
    }
}
