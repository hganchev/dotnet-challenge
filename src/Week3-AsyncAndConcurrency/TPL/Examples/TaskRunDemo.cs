using TPL.Concepts;

namespace TPL.Examples;

/// <summary>
/// Demonstrates Task.Run for offloading CPU-bound work to the thread pool,
/// contrasting it with async I/O and showing why it must not be used inside
/// library code.
/// </summary>
public static class TaskRunDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Task.Run Demo ===\n");

        await OffloadCpuWorkAsync();
        await ParallelFanOutAsync();
        await FireAndForgetPitfallAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Basic offload — keep the calling thread free while CPU work runs
    // -------------------------------------------------------------------------
    private static async Task OffloadCpuWorkAsync()
    {
        Console.WriteLine("--- Offload CPU work with Task.Run ---");
        Console.WriteLine($"  Calling thread: {Thread.CurrentThread.ManagedThreadId}");

        var result = await Task.Run(() =>
        {
            Console.WriteLine($"  Worker thread : {Thread.CurrentThread.ManagedThreadId}");
            return CpuBoundWork.Fibonacci(45); // takes a visible moment
        });

        Console.WriteLine($"  Fibonacci(45) = {result}");
        Console.WriteLine($"  Back on thread: {Thread.CurrentThread.ManagedThreadId}\n");
    }

    // -------------------------------------------------------------------------
    // 2. Fan-out — multiple CPU tasks running in parallel via Task.Run + WhenAll
    //    Rule: use Task.Run per work item; Task.WhenAll to collect results.
    // -------------------------------------------------------------------------
    private static async Task ParallelFanOutAsync()
    {
        Console.WriteLine("--- Fan-out with Task.Run + Task.WhenAll ---");

        var inputs = new[] { 40, 41, 42, 43, 44 };

        var sw = System.Diagnostics.Stopwatch.StartNew();

        var tasks = inputs.Select(n => Task.Run(() =>
            (Input: n, Result: CpuBoundWork.Fibonacci(n)))).ToArray();

        var results = await Task.WhenAll(tasks);
        sw.Stop();

        foreach (var (input, result) in results)
            Console.WriteLine($"  Fibonacci({input}) = {result}");

        Console.WriteLine($"  All done in {sw.ElapsedMilliseconds} ms (parallel, not sequential)\n");
    }

    // -------------------------------------------------------------------------
    // 3. Fire-and-forget pitfall — exceptions are silently lost
    //    Pattern: always await Task.Run; if fire-and-forget is intentional,
    //    attach a continuation that logs or faults fast.
    // -------------------------------------------------------------------------
    private static async Task FireAndForgetPitfallAsync()
    {
        Console.WriteLine("--- Fire-and-forget pitfall ---");

        // BAD: exception is lost — nothing handles this faulted task
        var _ = Task.Run(() => throw new InvalidOperationException("Unobserved!"));
        await Task.Delay(10); // give the task time to fault
        Console.WriteLine("  Unobserved exception: silently discarded (check TaskScheduler.UnobservedTaskException in production)");

        // BETTER: attach a continuation to log faults
        var safeTask = Task.Run(() => throw new InvalidOperationException("Observed"))
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Console.WriteLine($"  Observed fault: {t.Exception!.InnerException!.Message}");
            }, TaskContinuationOptions.OnlyOnFaulted);

        await safeTask;
        Console.WriteLine("  Best practice: always await Task.Run or handle the returned Task\n");
    }
}
