using LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;
using LetsTripTogether.InternalApi.Infrastructure.Clients;

namespace LetsTripTogether.InternalApi.Infrastructure.Services;

public class RedisService(RedisClient redisClient) : IRedisService
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
