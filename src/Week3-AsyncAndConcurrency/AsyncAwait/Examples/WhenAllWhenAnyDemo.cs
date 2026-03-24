namespace AsyncAwait.Examples;

/// <summary>
/// Demonstrates Task.WhenAll and Task.WhenAny for composing independent
/// async operations concurrently, including partial-failure handling.
/// </summary>
public static class WhenAllWhenAnyDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Task.WhenAll / Task.WhenAny Demo ===\n");

        await WhenAllSuccessAsync();
        await WhenAllPartialFailureAsync();
        await WhenAnyFirstWinsAsync();
        await WhenAnyTimeoutPatternAsync();
    }

    // -------------------------------------------------------------------------
    // 1. WhenAll — fan-out N tasks, collect all results
    // -------------------------------------------------------------------------
    private static async Task WhenAllSuccessAsync()
    {
        Console.WriteLine("--- Task.WhenAll — fan-out ---");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var results = await Task.WhenAll(
            FetchAsync("Service-A", 120),
            FetchAsync("Service-B",  80),
            FetchAsync("Service-C", 100));

        Console.WriteLine($"All done in {sw.ElapsedMilliseconds} ms (≈ max delay, not sum)");
        foreach (var r in results)
            Console.WriteLine($"  {r}");
        Console.WriteLine();
    }

    private static async Task<string> FetchAsync(string name, int delayMs)
    {
        await Task.Delay(delayMs).ConfigureAwait(false);
        return $"{name} responded in {delayMs} ms";
    }

    // -------------------------------------------------------------------------
    // 2. WhenAll — partial failure: AggregateException wraps all exceptions;
    //    'await' re-throws only the first. Inspect Task.Exception for all.
    // -------------------------------------------------------------------------
    private static async Task WhenAllPartialFailureAsync()
    {
        Console.WriteLine("--- Task.WhenAll — partial failure ---");

        var taskA = FetchAsync("OK-A", 50);
        var taskB = FailAsync("FAIL-B", 30);
        var taskC = FailAsync("FAIL-C", 40);

        try
        {
            await Task.WhenAll(taskA, taskB, taskC);
        }
        catch
        {
            // await re-throws only the first exception; inspect all:
            var allErrors = new[] { taskA, taskB, taskC }
                .Where(t => t.IsFaulted)
                .Select(t => t.Exception!.InnerException!.Message);

            Console.WriteLine("WhenAll faulted. All errors:");
            foreach (var err in allErrors)
                Console.WriteLine($"  {err}");
        }
        Console.WriteLine();
    }

    private static async Task<string> FailAsync(string name, int delayMs)
    {
        await Task.Delay(delayMs).ConfigureAwait(false);
        throw new InvalidOperationException($"{name} failed");
    }

    // -------------------------------------------------------------------------
    // 3. WhenAny — first task to finish wins (e.g. race multiple mirrors)
    // -------------------------------------------------------------------------
    private static async Task WhenAnyFirstWinsAsync()
    {
        Console.WriteLine("--- Task.WhenAny — first-wins race ---");

        var tasks = new[]
        {
            FetchAsync("Mirror-1", 200),
            FetchAsync("Mirror-2",  60),  // fastest
            FetchAsync("Mirror-3", 150),
        };

        var winner = await Task.WhenAny(tasks);
        Console.WriteLine($"Winner: {await winner}");
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 4. WhenAny — timeout pattern: race operation vs Task.Delay
    // -------------------------------------------------------------------------
    private static async Task WhenAnyTimeoutPatternAsync()
    {
        Console.WriteLine("--- Task.WhenAny — timeout pattern ---");
        using var cts = new CancellationTokenSource();

        var operation = SlowOperationAsync(cts.Token);
        var timeout   = Task.Delay(TimeSpan.FromMilliseconds(100));

        var completed = await Task.WhenAny(operation, timeout);

        if (completed == timeout)
        {
            await cts.CancelAsync(); // signal operation to stop
            Console.WriteLine("Operation timed out — cancelled\n");
        }
        else
        {
            Console.WriteLine($"Operation completed: {await operation}\n");
        }
    }

    private static async Task<string> SlowOperationAsync(CancellationToken ct)
    {
        await Task.Delay(500, ct).ConfigureAwait(false);
        return "slow result";
    }
}
