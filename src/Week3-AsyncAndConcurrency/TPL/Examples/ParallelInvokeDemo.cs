using TPL.Concepts;

namespace TPL.Examples;

/// <summary>
/// Demonstrates Parallel.Invoke for running a fixed set of independent
/// actions concurrently, and contrasts it with Task.WhenAll.
/// </summary>
public static class ParallelInvokeDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Parallel.Invoke Demo ===\n");

        BasicInvoke();
        InvokeVsWhenAll();
        InvokeWithCancellation();
    }

    // -------------------------------------------------------------------------
    // 1. Basic Parallel.Invoke — run three independent steps in parallel
    // -------------------------------------------------------------------------
    private static void BasicInvoke()
    {
        Console.WriteLine("--- Basic Parallel.Invoke ---");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        Parallel.Invoke(
            () => Step("LoadConfig",   30),
            () => Step("ConnectDb",    50),
            () => Step("WarmUpCache",  40));

        Console.WriteLine($"  All steps done in {sw.ElapsedMilliseconds} ms (≈ max(30,50,40) = 50 ms)\n");
    }

    private static void Step(string name, int durationMs)
    {
        CpuBoundWork.BusySpin(durationMs);
        Console.WriteLine($"  [{Thread.CurrentThread.ManagedThreadId}] {name} completed");
    }

    // -------------------------------------------------------------------------
    // 2. Parallel.Invoke vs Task.WhenAll
    //    Invoke: synchronous blocking call — blocks the caller until all done.
    //    WhenAll: async — caller is freed while tasks run.
    //    Use Invoke when you want synchronous start-up parallelism.
    // -------------------------------------------------------------------------
    private static void InvokeVsWhenAll()
    {
        Console.WriteLine("--- Parallel.Invoke vs Task.WhenAll semantics ---");

        Console.WriteLine("  Parallel.Invoke blocks the calling thread:");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Parallel.Invoke(
            () => CpuBoundWork.BusySpin(20),
            () => CpuBoundWork.BusySpin(20));
        Console.WriteLine($"  → Returned after {sw.ElapsedMilliseconds} ms (calling thread was blocked)");

        Console.WriteLine("  Task.WhenAll is async-compatible and doesn't block the calling thread.");
        Console.WriteLine("  Choose Parallel.Invoke for synchronous init code only.\n");
    }

    // -------------------------------------------------------------------------
    // 3. AggregateException from Parallel.Invoke
    //    All exceptions are collected and re-thrown as AggregateException.
    // -------------------------------------------------------------------------
    private static void InvokeWithCancellation()
    {
        Console.WriteLine("--- Parallel.Invoke exception aggregation ---");

        try
        {
            Parallel.Invoke(
                () => CpuBoundWork.BusySpin(10),
                () => throw new InvalidOperationException("Step A failed"),
                () => throw new ArgumentException("Step B failed"));
        }
        catch (AggregateException ae)
        {
            Console.WriteLine($"  {ae.InnerExceptions.Count} exception(s) from Parallel.Invoke:");
            foreach (var ex in ae.InnerExceptions)
                Console.WriteLine($"    [{ex.GetType().Name}] {ex.Message}");
        }
        Console.WriteLine();
    }
}
