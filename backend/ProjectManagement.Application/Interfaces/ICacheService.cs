using Microsoft.Extensions.Caching.Distributed;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task InvalidateProjectCache(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
}
