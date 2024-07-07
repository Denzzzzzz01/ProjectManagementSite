using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Interfaces;
using System.Text.Json;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var cachedData = await _cache.GetStringAsync(key, ct);
        if (cachedData == null)
            return default;

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken ct = default)
    {
        var serializedData = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedData, options, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public async Task InvalidateProjectCache(Guid projectId, Guid userId, CancellationToken ct = default)
    {
        var projectCacheKey = $"Project_{projectId}_{userId}";
        await _cache.RemoveAsync(projectCacheKey, ct);
        _logger.LogInformation("Invalidated cache for project {ProjectId} and user {UserId}", projectId, userId);
    }
}
