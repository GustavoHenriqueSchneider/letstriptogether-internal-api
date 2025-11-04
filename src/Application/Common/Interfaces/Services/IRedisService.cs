namespace LetsTripTogether.InternalApi.Application.Common.Interfaces.Services;

public interface IRedisService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value, int ttlSeconds);
    Task DeleteAsync(string key);
}
