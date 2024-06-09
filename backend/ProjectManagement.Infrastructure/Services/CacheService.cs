using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using ProjectManagement.Application.Interfaces;
using System.Text.Json;

namespace ProjectManagement.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await _cache.GetAsync(key, cancellationToken);
        if (cachedData == null)
        {
            return default;
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var jsonString = Encoding.UTF8.GetString(cachedData);
        return JsonSerializer.Deserialize<T>(jsonString, options);
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
    {
        var optionsJson = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var jsonString = JsonSerializer.Serialize(value, optionsJson);
        var encodedData = Encoding.UTF8.GetBytes(jsonString);

        await _cache.SetAsync(key, encodedData, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}