using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ScalabilityAndCaching.Caching;

namespace ScalabilityAndCaching.Examples;

/// <summary>Demonstrates cache-aside with <see cref="IMemoryCache"/>, the same pattern over
/// <see cref="IDistributedCache"/>, and stampede prevention under concurrent load.</summary>
public static class CachingDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Caching: Cache-Aside (IMemoryCache) ===");
        var catalog = new ProductCatalog();
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var cacheAside = new CacheAsideProductService(memoryCache, catalog);

        var first = await cacheAside.GetProductAsync(1);
        var second = await cacheAside.GetProductAsync(1);
        Console.WriteLine($"First call  -> {first} (db calls so far: {catalog.DbCallCount})");
        Console.WriteLine($"Second call -> {second} (db calls so far: {catalog.DbCallCount}, expected 1: cache hit)");

        cacheAside.Invalidate(1);
        Console.WriteLine("Explicitly invalidated the cache entry for product 1.");

        Console.WriteLine("\n=== Caching: Distributed Cache (IDistributedCache, Redis-compatible API) ===");
        var distributedCatalog = new ProductCatalog();
        var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        var distributedService = new DistributedCacheProductService(distributedCache, distributedCatalog);
        await distributedService.GetProductAsync(2);
        await distributedService.GetProductAsync(2);
        Console.WriteLine($"Distributed cache demo db calls: {distributedCatalog.DbCallCount} (expected 1: second call hit the cache)");

        Console.WriteLine("\n=== Cache Stampede Prevention ===");
        var unprotectedCatalog = new ProductCatalog();
        using var unprotectedCache = new MemoryCache(new MemoryCacheOptions());
        var unprotectedTasks = Enumerable.Range(0, 10)
            .Select(_ => GetOrLoadWithoutLockAsync(unprotectedCache, unprotectedCatalog));
        await Task.WhenAll(unprotectedTasks);
        Console.WriteLine($"Without stampede prevention: {unprotectedCatalog.DbCallCount} concurrent DB calls for the same key");

        var protectedCatalog = new ProductCatalog();
        using var protectedCache = new MemoryCache(new MemoryCacheOptions());
        var protectedService = new StampedePreventionCacheService(protectedCache, protectedCatalog);
        var protectedTasks = Enumerable.Range(0, 10).Select(_ => protectedService.GetProductAsync(1));
        await Task.WhenAll(protectedTasks);
        Console.WriteLine($"With stampede prevention:    {protectedCatalog.DbCallCount} DB call for the same key under identical concurrent load");
    }

    // Naive cache-aside with no locking — every concurrent miss falls through to the "database" at once.
    private static async Task GetOrLoadWithoutLockAsync(IMemoryCache cache, ProductCatalog catalog)
    {
        const string cacheKey = "product:1";
        if (cache.TryGetValue(cacheKey, out Product? _))
        {
            return;
        }

        var product = await catalog.GetProductFromDatabaseAsync(1);
        cache.Set(cacheKey, product, TimeSpan.FromSeconds(5));
    }
}
