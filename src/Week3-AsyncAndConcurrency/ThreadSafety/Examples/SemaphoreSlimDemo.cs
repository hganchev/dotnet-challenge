namespace ThreadSafety.Examples;

/// <summary>
/// Demonstrates SemaphoreSlim for async-compatible throttling:
/// limiting concurrent access to a resource, rate-limiting, and
/// async mutual exclusion (as a binary semaphore).
/// </summary>
public static class SemaphoreSlimDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== SemaphoreSlim Demo ===\n");

        await ThrottleConcurrentRequestsAsync();
        await AsyncMutualExclusionAsync();
        await RateLimitingDemo();
    }

    // -------------------------------------------------------------------------
    // 1. Throttle concurrent I/O — allow at most N simultaneous operations
    //    Classic use case: limit parallel HTTP requests to avoid overwhelming a server.
    // -------------------------------------------------------------------------
    private static async Task ThrottleConcurrentRequestsAsync()
    {
        Console.WriteLine("--- Throttle concurrent requests (max 3 at a time) ---");

        using var semaphore = new SemaphoreSlim(initialCount: 3, maxCount: 3);
        var tasks = Enumerable.Range(1, 9).Select(async i =>
        {
            await semaphore.WaitAsync();          // acquire a slot
            try
            {
                Console.WriteLine($"  [{i,2}] started  (slots remaining: {semaphore.CurrentCount})");
                await Task.Delay(50).ConfigureAwait(false); // simulate I/O
                Console.WriteLine($"  [{i,2}] finished");
            }
            finally
            {
                semaphore.Release();              // always release in finally
            }
        });

        await Task.WhenAll(tasks);
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 2. Async mutual exclusion — SemaphoreSlim(1,1) as an async-safe lock
    //    Use this instead of lock() when you need to await inside the critical section.
    // -------------------------------------------------------------------------
    private static async Task AsyncMutualExclusionAsync()
    {
        Console.WriteLine("--- SemaphoreSlim(1,1) as async mutex ---");

        using var mutex = new SemaphoreSlim(1, 1);
        var sharedList = new List<int>();

        async Task AppendAsync(int value)
        {
            await mutex.WaitAsync();
            try
            {
                // Safe to await inside the critical section
                await Task.Delay(5).ConfigureAwait(false);
                sharedList.Add(value);
            }
            finally
            {
                mutex.Release();
            }
        }

        await Task.WhenAll(Enumerable.Range(1, 8).Select(AppendAsync));

        Console.WriteLine($"  List count: {sharedList.Count} (expected: 8)");
        Console.WriteLine($"  Contents  : [{string.Join(", ", sharedList.OrderBy(x => x))}]\n");
    }

    // -------------------------------------------------------------------------
    // 3. Rate limiting — allow at most N operations per time window
    //    Refill the semaphore on a timer to implement a token-bucket style rate limiter.
    // -------------------------------------------------------------------------
    private static async Task RateLimitingDemo()
    {
        Console.WriteLine("--- Rate limiting (3 ops per 100 ms window) ---");

        const int MaxPerWindow = 3;
        const int WindowMs    = 100;
        using var cts         = new CancellationTokenSource();
        using var semaphore   = new SemaphoreSlim(MaxPerWindow, MaxPerWindow);

        // Refill task: top up the semaphore every window
        var refillTask = Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(WindowMs, cts.Token).ConfigureAwait(false);
                var toRelease = MaxPerWindow - semaphore.CurrentCount;
                if (toRelease > 0)
                    semaphore.Release(toRelease);
            }
        }, cts.Token);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        for (var i = 1; i <= 7; i++)
        {
            await semaphore.WaitAsync(cts.Token);
            Console.WriteLine($"  Op {i} at {sw.ElapsedMilliseconds,4} ms");
            await Task.Delay(10).ConfigureAwait(false); // simulate quick work
        }

        await cts.CancelAsync();
        try { await refillTask; } catch (OperationCanceledException) { }

        Console.WriteLine("  (Ops 1-3 run immediately; ops 4-6 wait for the next window)\n");
    }
}
