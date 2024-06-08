using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class CacheTestController : ControllerBase
{
    private readonly IDistributedCache _cache;

    public CacheTestController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var cacheKey = "TheTime";
        var existingTime = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(existingTime))
        {
            return Ok(new { CachedTime = existingTime });
        }
        var currentTime = DateTime.UtcNow.ToString();
        await _cache.SetStringAsync(cacheKey, currentTime, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        });
        return Ok(new { CurrentTime = currentTime });
    }
}
