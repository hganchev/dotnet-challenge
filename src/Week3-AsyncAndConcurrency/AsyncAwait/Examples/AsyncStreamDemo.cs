namespace AsyncAwait.Examples;

/// <summary>
/// Demonstrates IAsyncEnumerable&lt;T&gt; and the await foreach pattern
/// for streaming async sequences without buffering all results.
/// </summary>
public static class AsyncStreamDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Async Streams (IAsyncEnumerable<T>) Demo ===\n");

        await BasicStreamAsync();
        await StreamWithCancellationAsync();
        await StreamTransformPipelineAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Produce and consume a stream with await foreach
    // -------------------------------------------------------------------------
    private static async Task BasicStreamAsync()
    {
        Console.WriteLine("--- Basic async stream ---");

        await foreach (var item in GenerateItemsAsync(count: 5))
        {
            Console.WriteLine($"  Received: {item}");
        }
        Console.WriteLine();
    }

    /// <summary>Yields items one at a time with a simulated async delay.</summary>
    private static async IAsyncEnumerable<string> GenerateItemsAsync(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            await Task.Delay(20).ConfigureAwait(false); // simulate I/O per item
            yield return $"item-{i}";
        }
    }

    // -------------------------------------------------------------------------
    // 2. Passing CancellationToken through an async iterator
    //    Use [EnumeratorCancellation] on the token parameter so callers can
    //    pass it via WithCancellation() or directly.
    // -------------------------------------------------------------------------
    private static async Task StreamWithCancellationAsync()
    {
        Console.WriteLine("--- Async stream with cancellation ---");
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(120));

        try
        {
            await foreach (var page in FetchPagesAsync(cts.Token))
            {
                Console.WriteLine($"  Page: {page}");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  Stream cancelled before all pages were fetched\n");
        }
    }

    private static async IAsyncEnumerable<string> FetchPagesAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        for (var page = 1; page <= 10; page++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(40, ct).ConfigureAwait(false);
            yield return $"Page {page}";
        }
    }

    // -------------------------------------------------------------------------
    // 3. Stream pipeline — produce → filter → transform using await foreach
    //    Compare to LINQ: no buffering; each stage processes items as they arrive.
    // -------------------------------------------------------------------------
    private static async Task StreamTransformPipelineAsync()
    {
        Console.WriteLine("--- Stream pipeline (filter + transform) ---");

        await foreach (var result in TransformAsync(FilterAsync(GenerateNumbersAsync())))
        {
            Console.WriteLine($"  {result}");
        }
        Console.WriteLine();
    }

    private static async IAsyncEnumerable<int> GenerateNumbersAsync()
    {
        for (var n = 1; n <= 10; n++)
        {
            await Task.Delay(5).ConfigureAwait(false);
            yield return n;
        }
    }

    private static async IAsyncEnumerable<int> FilterAsync(IAsyncEnumerable<int> source)
    {
        await foreach (var n in source.ConfigureAwait(false))
        {
            if (n % 2 == 0) // keep even numbers
                yield return n;
        }
    }

    private static async IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<int> source)
    {
        await foreach (var n in source.ConfigureAwait(false))
        {
            await Task.Delay(5).ConfigureAwait(false);
            yield return $"{n} squared = {n * n}";
        }
    }
}
