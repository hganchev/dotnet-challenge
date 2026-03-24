using TPL.Concepts;

namespace TPL.Examples;

/// <summary>
/// Demonstrates PLINQ (.AsParallel()) for data-parallel LINQ queries,
/// covering ordering, degree of parallelism, exception handling,
/// and when NOT to use PLINQ.
/// </summary>
public static class PlinqDemo
{
    public static void Run()
    {
        Console.WriteLine("=== PLINQ Demo ===\n");

        BasicPlinq();
        OrderedPlinq();
        PlinqWithDop();
        PlinqExceptionHandling();
        WhenNotToUsePlinq();
    }

    // -------------------------------------------------------------------------
    // 1. Basic PLINQ — simply add .AsParallel()
    //    Results are unordered by default; partition + process in parallel.
    // -------------------------------------------------------------------------
    private static void BasicPlinq()
    {
        Console.WriteLine("--- Basic .AsParallel() ---");

        var numbers = Enumerable.Range(1, 10).ToArray();

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Sequential
        var seqResult = numbers
            .Select(n => CpuBoundWork.Fibonacci(38 + n))
            .ToArray();

        var seqMs = sw.ElapsedMilliseconds;
        sw.Restart();

        // Parallel
        var parResult = numbers
            .AsParallel()
            .Select(n => CpuBoundWork.Fibonacci(38 + n))
            .ToArray();

        var parMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"  Sequential : {seqMs} ms — {seqResult.Length} results");
        Console.WriteLine($"  Parallel   : {parMs} ms — {parResult.Length} results");
        Console.WriteLine($"  Speedup    : ~{(double)seqMs / Math.Max(parMs, 1):F1}x\n");
    }

    // -------------------------------------------------------------------------
    // 2. AsOrdered() — preserves input order at extra synchronisation cost
    // -------------------------------------------------------------------------
    private static void OrderedPlinq()
    {
        Console.WriteLine("--- AsOrdered() ---");

        var source = Enumerable.Range(1, 8).ToArray();

        var unordered = source.AsParallel()
            .Select(n => n * n)
            .ToArray();

        var ordered = source.AsParallel()
            .AsOrdered()
            .Select(n => n * n)
            .ToArray();

        Console.WriteLine($"  Unordered: [{string.Join(", ", unordered)}]");
        Console.WriteLine($"  Ordered  : [{string.Join(", ", ordered)}]");
        Console.WriteLine("  (Unordered output varies per run; Ordered is always 1,4,9,16,…)\n");
    }

    // -------------------------------------------------------------------------
    // 3. WithDegreeOfParallelism — cap threads to avoid scheduler overload
    // -------------------------------------------------------------------------
    private static void PlinqWithDop()
    {
        Console.WriteLine("--- WithDegreeOfParallelism ---");

        var dop = Math.Max(2, Environment.ProcessorCount / 2);

        var result = Enumerable.Range(1, 8)
            .AsParallel()
            .WithDegreeOfParallelism(dop)
            .Select(n => CpuBoundWork.Fibonacci(38 + n))
            .ToArray();

        Console.WriteLine($"  DOP={dop}, computed {result.Length} Fibonacci values");
        Console.WriteLine($"  First three results: {result[0]}, {result[1]}, {result[2]}\n");
    }

    // -------------------------------------------------------------------------
    // 4. PLINQ exception handling — AggregateException wraps all exceptions
    // -------------------------------------------------------------------------
    private static void PlinqExceptionHandling()
    {
        Console.WriteLine("--- PLINQ exception handling ---");

        try
        {
            var _ = Enumerable.Range(0, 10)
                .AsParallel()
                .Select(n => n == 5 ? throw new InvalidOperationException($"Bad value: {n}") : n * 2)
                .ToArray();
        }
        catch (AggregateException ae)
        {
            foreach (var inner in ae.InnerExceptions)
                Console.WriteLine($"  Exception caught: {inner.Message}");
        }
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 5. When NOT to use PLINQ — small collections pay overhead without gain
    // -------------------------------------------------------------------------
    private static void WhenNotToUsePlinq()
    {
        Console.WriteLine("--- When NOT to use PLINQ (small collection overhead) ---");

        var small = Enumerable.Range(1, 5).ToArray();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var seqResult = small.Select(n => n * n).ToArray();
        var seqUs = sw.Elapsed.TotalMicroseconds;

        sw.Restart();
        var parResult = small.AsParallel().Select(n => n * n).ToArray();
        var parUs = sw.Elapsed.TotalMicroseconds;

        Console.WriteLine($"  Sequential (5 items): {seqUs:F0} µs");
        Console.WriteLine($"  Parallel   (5 items): {parUs:F0} µs");
        Console.WriteLine("  → PLINQ overhead dominates for tiny workloads; always benchmark before adopting.\n");
    }
}
