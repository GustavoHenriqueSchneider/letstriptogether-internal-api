using WebApi.Clients.Interfaces;
using WebApi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace WebApi.Services.Implementations;

public class RedisService(IRedisClient redisClient, ILogger<RedisService> logger) : IRedisService
{
	public async Task<string?> GetAsync(string key)
	{
		try
		{
			var value = await redisClient.Database.StringGetAsync(key);
			logger.LogInformation("Redis GET {Key} hit={Hit}", key, value.HasValue);
			return value;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error on Redis GET {Key}", key);
			throw;
		}
	}

	public async Task SetAsync(string key, string value, int ttlSeconds)
	{
		try
		{
			await redisClient.Database.StringSetAsync(key, value, TimeSpan.FromSeconds(ttlSeconds));
			logger.LogInformation("Redis SET {Key} ttl={TtlSeconds}s", key, ttlSeconds);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error on Redis SET {Key}", key);
			throw;
		}
	}

	public async Task DeleteAsync(string key)
	{
		try
		{
			await redisClient.Database.KeyDeleteAsync(key);
			logger.LogInformation("Redis DEL {Key}", key);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error on Redis DEL {Key}", key);
			throw;
		}
	}
}
