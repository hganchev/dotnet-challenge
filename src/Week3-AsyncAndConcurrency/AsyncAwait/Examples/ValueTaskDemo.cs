namespace AsyncAwait.Examples;

/// <summary>
/// Demonstrates ValueTask&lt;T&gt; and its allocation-free benefit for hot-path
/// async methods that frequently complete synchronously (e.g. cache hits).
/// </summary>
public static class ValueTaskDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== ValueTask<T> Demo ===\n");

        await WhenToPreferValueTaskAsync();
        await CacheLookupPatternAsync();
        await ValueTaskPitfallsAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Task<T> vs ValueTask<T> — allocation comparison
    //    Task<T> always allocates a heap object even when the result is known
    //    synchronously. ValueTask<T> avoids the allocation in the sync-complete path.
    // -------------------------------------------------------------------------
    private static async Task WhenToPreferValueTaskAsync()
    {
        Console.WriteLine("--- Task<T> vs ValueTask<T> ---");

        // Task<T>: always allocates a Task object
        var taskResult = await GetFromCacheAsTask("key1");
        Console.WriteLine($"  Task<T> result: {taskResult}");

        // ValueTask<T>: zero allocation when result is already available
        var vtResult = await GetFromCacheAsValueTask("key1");
        Console.WriteLine($"  ValueTask<T> result: {vtResult}");

        Console.WriteLine("  (Difference is visible in GC allocation profiling, not the output)\n");
    }

    private static Task<string> GetFromCacheAsTask(string key)
    {
        // Even though the result is synchronously available,
        // Task.FromResult always allocates a Task<string> object.
        return Task.FromResult($"cached:{key}");
    }

    private static ValueTask<string> GetFromCacheAsValueTask(string key)
    {
        // ValueTask<T> wraps the value directly — no heap object allocated.
        return new ValueTask<string>($"cached:{key}");
    }

    // -------------------------------------------------------------------------
    // 2. Real cache-hit / cache-miss pattern
    //    Synchronous path (hit)  → ValueTask<T> with no allocation
    //    Asynchronous path (miss) → ValueTask<T> wrapping an actual Task<T>
    // -------------------------------------------------------------------------
    private static async Task CacheLookupPatternAsync()
    {
        Console.WriteLine("--- Cache hit/miss pattern ---");

        var cache = new SimpleCache();

        // First call: cache miss → async fetch
        var v1 = await cache.GetValueAsync("user:42");
        Console.WriteLine($"  Miss → fetched: {v1}");

        // Second call: cache hit → synchronous return, zero allocation
        var v2 = await cache.GetValueAsync("user:42");
        Console.WriteLine($"  Hit  → returned: {v2}\n");
    }

    // -------------------------------------------------------------------------
    // 3. ValueTask pitfalls
    //    - Do NOT await a ValueTask<T> more than once.
    //    - Do NOT store and re-use a ValueTask<T> variable.
    //    - If you need multiple awaits, call .AsTask() first.
    // -------------------------------------------------------------------------
    private static async Task ValueTaskPitfallsAsync()
    {
        Console.WriteLine("--- ValueTask pitfalls ---");

        var vt = GetSingleUseValueTask();

        // CORRECT: await once immediately
        var result = await vt;
        Console.WriteLine($"  First await (correct): {result}");

        // If you need to await multiple times or share the result,
        // convert to Task<T> first:
        var asTask = GetSingleUseValueTask().AsTask();
        var r1 = await asTask;
        var r2 = await asTask; // safe: Task can be awaited multiple times
        Console.WriteLine($"  AsTask() — r1={r1}, r2={r2}");
        Console.WriteLine();
    }

    private static async ValueTask<int> GetSingleUseValueTask()
    {
        await Task.Delay(1).ConfigureAwait(false);
        return 42;
    }
}

// ---------------------------------------------------------------------------
// Supporting type — simple in-memory cache with async population
// ---------------------------------------------------------------------------

file sealed class SimpleCache
{
    private readonly Dictionary<string, string> _store = [];

    public ValueTask<string> GetValueAsync(string key)
    {
        // Synchronous hit — no Task allocation
        if (_store.TryGetValue(key, out var cached))
            return new ValueTask<string>(cached);

        // Async miss — fall through to actual fetch
        return new ValueTask<string>(FetchAndStoreAsync(key));
    }

    private async Task<string> FetchAndStoreAsync(string key)
    {
        await Task.Delay(30).ConfigureAwait(false); // simulate DB / API call
        var value = $"data-for-{key}";
        _store[key] = value;
        return value;
    }
}
