using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace ScalabilityAndCaching.Caching;

/// <summary>Cache-aside over <see cref="IDistributedCache"/>. This demo wires up
/// <c>MemoryDistributedCache</c> (an in-process implementation of the same interface) purely to avoid
/// an external dependency; swapping in a real Redis-backed cache in production only requires
/// registering `AddStackExchangeRedisCache(...)` instead — none of this class's code would change.</summary>
public class DistributedCacheProductService
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5),
    };

    private readonly IDistributedCache _cache;
    private readonly ProductCatalog _catalog;

    public DistributedCacheProductService(IDistributedCache cache, ProductCatalog catalog)
    {
        _cache = cache;
        _catalog = catalog;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"product:{id}";
        var cachedBytes = await _cache.GetAsync(cacheKey);
        if (cachedBytes is not null)
        {
            return JsonSerializer.Deserialize<Product>(cachedBytes);
        }

        var product = await _catalog.GetProductFromDatabaseAsync(id);
        if (product is not null)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(product);
            await _cache.SetAsync(cacheKey, bytes, CacheOptions);
        }

        return product;
    }
}
