using Application.Common.Interfaces.Services;
using Infrastructure.Clients;

namespace Infrastructure.Services;

public class RedisService(IRedisClient redisClient) : IRedisService
{
    public async Task<string?> GetAsync(string key)
    {
        return await redisClient.Database.StringGetAsync(key);
    }

    public async Task SetAsync(string key, string value, int ttlSeconds)
    {
        await redisClient.Database.StringSetAsync(key, value, TimeSpan.FromSeconds(ttlSeconds));
    }

    public async Task DeleteAsync(string key)
    {
        await redisClient.Database.KeyDeleteAsync(key);
    }
}
