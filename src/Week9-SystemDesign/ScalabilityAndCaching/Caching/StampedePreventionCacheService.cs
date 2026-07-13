using Microsoft.Extensions.Caching.Memory;

namespace ScalabilityAndCaching.Caching;

/// <summary>Prevents a cache stampede (thundering herd): when many concurrent callers miss the cache
/// for the same key at once, only the first caller actually queries the slow store — the rest wait on
/// a lock and then reuse the freshly-populated cache entry.</summary>
public class StampedePreventionCacheService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(5);

    private readonly IMemoryCache _cache;
    private readonly ProductCatalog _catalog;
    // A single semaphore keeps this demo simple; a production system would use a
    // ConcurrentDictionary<string, SemaphoreSlim> keyed per cache key so unrelated keys don't block each other.
    private readonly SemaphoreSlim _lock = new(1, 1);

    public StampedePreventionCacheService(IMemoryCache cache, ProductCatalog catalog)
    {
        _cache = cache;
        _catalog = catalog;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";
        if (_cache.TryGetValue(cacheKey, out Product? cached))
        {
            return cached;
        }

        await _lock.WaitAsync();
        try
        {
            // Double-checked locking: another caller may have already populated the cache while
            // we were waiting for the lock. This second check is what actually prevents the stampede.
            if (_cache.TryGetValue(cacheKey, out cached))
            {
                return cached;
            }

            var product = await _catalog.GetProductFromDatabaseAsync(id);
            if (product is not null)
            {
                _cache.Set(cacheKey, product, CacheTtl);
            }

            return product;
        }
        finally
        {
            _lock.Release();
        }
    }
}
