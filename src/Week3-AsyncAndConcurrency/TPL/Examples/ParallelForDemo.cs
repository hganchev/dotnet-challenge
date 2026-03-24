using System.Collections.Concurrent;
using TPL.Concepts;

namespace TPL.Examples;

/// <summary>
/// Demonstrates Parallel.For and Parallel.ForEach for data parallelism,
/// including MaxDegreeOfParallelism, thread-safe aggregation, and
/// cancellation via ParallelOptions.
/// </summary>
public static class ParallelForDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Parallel.For / Parallel.ForEach Demo ===\n");

        BasicParallelFor();
        ParallelForEachWithMaxDop();
        ParallelForEachWithLocalState();
        await ParallelWithCancellationAsync();
        WhyMaxDopMatters();
    }

    // -------------------------------------------------------------------------
    // 1. Basic Parallel.For — run independent iterations in parallel
    // -------------------------------------------------------------------------
    private static void BasicParallelFor()
    {
        Console.WriteLine("--- Basic Parallel.For ---");
        var results = new long[6];

        Parallel.For(0, results.Length, i =>
        {
            results[i] = CpuBoundWork.Fibonacci(40 + i);
        });

        for (var i = 0; i < results.Length; i++)
            Console.WriteLine($"  Fibonacci({40 + i}) = {results[i]}");
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 2. Parallel.ForEach with MaxDegreeOfParallelism
    //    Always cap DOP to avoid thread-pool starvation.
    // -------------------------------------------------------------------------
    private static void ParallelForEachWithMaxDop()
    {
        Console.WriteLine("--- Parallel.ForEach with MaxDegreeOfParallelism ---");

        var items = CpuBoundWork.CreateBatch(12);
        var processedBy = new ConcurrentBag<int>();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2)
        };

        Console.WriteLine($"  Logical CPUs: {Environment.ProcessorCount}, MaxDOP: {options.MaxDegreeOfParallelism}");

        var sw = System.Diagnostics.Stopwatch.StartNew();
        Parallel.ForEach(items, options, item =>
        {
            CpuBoundWork.BusySpin(durationMs: 10); // 10 ms of CPU work per item
            processedBy.Add(Thread.CurrentThread.ManagedThreadId);
        });
        sw.Stop();

        var uniqueThreads = processedBy.Distinct().Count();
        Console.WriteLine($"  Processed {items.Length} items in {sw.ElapsedMilliseconds} ms across {uniqueThreads} thread(s)\n");
    }

    // -------------------------------------------------------------------------
    // 3. Parallel.ForEach with thread-local state
    //    Use localInit / localFinally to accumulate per-thread partial sums
    //    and merge them at the end — avoids contention on a shared counter.
    // -------------------------------------------------------------------------
    private static void ParallelForEachWithLocalState()
    {
        Console.WriteLine("--- Parallel.ForEach with thread-local aggregation ---");

        var numbers = Enumerable.Range(1, 1_000_000).ToArray();
        long totalSum = 0;

        Parallel.ForEach(
            source: numbers,
            localInit: () => 0L,
            body: (n, _, localSum) => localSum + n,
            localFinally: localSum => Interlocked.Add(ref totalSum, localSum));

        Console.WriteLine($"  Sum 1..1,000,000 = {totalSum:N0} (expected: {(long)1_000_000 * 1_000_001 / 2:N0})\n");
    }

    // -------------------------------------------------------------------------
    // 4. Parallel.For with cancellation via ParallelOptions.CancellationToken
    // -------------------------------------------------------------------------
    private static async Task ParallelWithCancellationAsync()
    {
        Console.WriteLine("--- Parallel.ForEach with cancellation ---");

        using var cts = new CancellationTokenSource();

        // Cancel after 60 ms
        _ = Task.Run(async () =>
        {
            await Task.Delay(60);
            await cts.CancelAsync();
        });

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = cts.Token
        };

        try
        {
            Parallel.For(0, 50, options, i =>
            {
                CpuBoundWork.BusySpin(durationMs: 20);
                Console.Write($" {i}");
            });
            Console.WriteLine("\n  All iterations completed");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\n  Parallel loop cancelled mid-run\n");
        }
    }

    // -------------------------------------------------------------------------
    // 5. Why MaxDOP matters — uncapped vs capped throughput comparison
    // -------------------------------------------------------------------------
    private static void WhyMaxDopMatters()
    {
        Console.WriteLine("--- Why MaxDegreeOfParallelism matters ---");

        var items = CpuBoundWork.CreateBatch(32);

        var swUncapped = System.Diagnostics.Stopwatch.StartNew();
        Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = -1 }, _ =>
            CpuBoundWork.BusySpin(5));
        swUncapped.Stop();

        var swCapped = System.Diagnostics.Stopwatch.StartNew();
        Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, _ =>
            CpuBoundWork.BusySpin(5));
        swCapped.Stop();

        Console.WriteLine($"  Uncapped (MaxDOP=-1) : {swUncapped.ElapsedMilliseconds} ms");
        Console.WriteLine($"  Capped   (MaxDOP={Environment.ProcessorCount,2})  : {swCapped.ElapsedMilliseconds} ms");
        Console.WriteLine($"  (On a {Environment.ProcessorCount}-core machine the difference narrows; on a heavily loaded system capping prevents starvation)\n");
    }
}
