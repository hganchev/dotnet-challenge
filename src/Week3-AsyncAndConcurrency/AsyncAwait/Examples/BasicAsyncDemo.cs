namespace AsyncAwait.Examples;

/// <summary>
/// Demonstrates core async/await patterns: returning Task/Task&lt;T&gt;,
/// awaiting multiple operations, and handling exceptions in async code.
/// </summary>
public static class BasicAsyncDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Basic Async / Await Demo ===\n");

        await SimpleTaskDemoAsync();
        await ReturningValueDemoAsync();
        await ExceptionHandlingDemoAsync();
        await SequentialVsConcurrentAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Basic async method returning Task (no result)
    // -------------------------------------------------------------------------
    private static async Task SimpleTaskDemoAsync()
    {
        Console.WriteLine("--- Simple async Task ---");
        Console.WriteLine("Before await");
        await Task.Delay(50);
        Console.WriteLine("After await (continuation ran)\n");
    }

    // -------------------------------------------------------------------------
    // 2. Async method returning Task<T>
    // -------------------------------------------------------------------------
    private static async Task ReturningValueDemoAsync()
    {
        Console.WriteLine("--- Async Task<T> ---");
        var result = await FetchDataAsync("https://example.com");
        Console.WriteLine($"Fetched: {result}\n");
    }

    private static async Task<string> FetchDataAsync(string url)
    {
        // Simulate network I/O
        await Task.Delay(30);
        return $"[data from {url}]";
    }

    // -------------------------------------------------------------------------
    // 3. Exception handling — try/catch wraps await
    // -------------------------------------------------------------------------
    private static async Task ExceptionHandlingDemoAsync()
    {
        Console.WriteLine("--- Exception Handling ---");
        try
        {
            await FailingOperationAsync();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Caught expected exception: {ex.Message}");
        }
        Console.WriteLine();
    }

    private static async Task FailingOperationAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("Something went wrong inside async method.");
    }

    // -------------------------------------------------------------------------
    // 4. Sequential vs concurrent awaits — shows the timing difference
    // -------------------------------------------------------------------------
    private static async Task SequentialVsConcurrentAsync()
    {
        Console.WriteLine("--- Sequential vs Concurrent ---");

        // Sequential: total ≈ 100ms + 100ms = 200ms
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await Step("A", 100);
        await Step("B", 100);
        Console.WriteLine($"Sequential:  {sw.ElapsedMilliseconds} ms");

        // Concurrent: start both, then await — total ≈ max(100,100) = 100ms
        sw.Restart();
        var taskA = Step("C", 100);
        var taskB = Step("D", 100);
        await taskA;
        await taskB;
        Console.WriteLine($"Concurrent:  {sw.ElapsedMilliseconds} ms\n");
    }

    private static async Task Step(string name, int delayMs)
    {
        await Task.Delay(delayMs);
        Console.WriteLine($"  Step {name} done");
    }
}
