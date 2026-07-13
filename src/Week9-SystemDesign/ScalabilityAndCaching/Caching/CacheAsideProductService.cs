using Microsoft.Extensions.Caching.Memory;

namespace ScalabilityAndCaching.Caching;

/// <summary>Classic cache-aside pattern backed by <see cref="IMemoryCache"/>: check the cache first,
/// fall back to the slow store on a miss, then populate the cache with a TTL for next time.</summary>
public class CacheAsideProductService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(5);

    private readonly IMemoryCache _cache;
    private readonly ProductCatalog _catalog;

    public CacheAsideProductService(IMemoryCache cache, ProductCatalog catalog)
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

        var product = await _catalog.GetProductFromDatabaseAsync(id);
        if (product is not null)
        {
            var entryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheTtl)
                .RegisterPostEvictionCallback((key, _, reason, _) =>
                    Console.WriteLine($"  [cache-aside] evicted '{key}' (reason: {reason})"));
            _cache.Set(cacheKey, product, entryOptions);
        }

        return product;
    }

    // Explicit eviction: invalidate a key the moment the underlying data changes,
    // instead of waiting for the TTL to expire.
    public void Invalidate(int id) => _cache.Remove($"product:{id}");
}
