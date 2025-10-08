namespace WebApi.Services.Interfaces;

public interface IRedisService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value, int ttlSeconds);
    Task DeleteAsync(string key);
}
