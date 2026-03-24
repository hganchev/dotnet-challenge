namespace AsyncAwait.Examples;

/// <summary>
/// Demonstrates cooperative cancellation with CancellationToken:
/// propagating tokens through the call chain, observing cancellation,
/// and using linked token sources.
/// </summary>
public static class CancellationDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Cancellation Demo ===\n");

        await BasicCancellationAsync();
        await CancelMidwayAsync();
        await LinkedTokenSourceAsync();
        await CooperativeCheckAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Cancel before the operation starts
    // -------------------------------------------------------------------------
    private static async Task BasicCancellationAsync()
    {
        Console.WriteLine("--- Cancel before start ---");
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // already cancelled

        try
        {
            await Task.Delay(500, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Task cancelled before it started (OperationCanceledException)\n");
        }
    }

    // -------------------------------------------------------------------------
    // 2. Cancel mid-way through a long operation
    // -------------------------------------------------------------------------
    private static async Task CancelMidwayAsync()
    {
        Console.WriteLine("--- Cancel mid-way ---");
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));

        try
        {
            await LongRunningOperationAsync(cts.Token);
            Console.WriteLine("Completed (unexpected)");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled mid-way\n");
        }
    }

    private static async Task LongRunningOperationAsync(CancellationToken ct)
    {
        for (var i = 0; i < 5; i++)
        {
            ct.ThrowIfCancellationRequested();
            Console.WriteLine($"  Working... step {i + 1}/5");
            await Task.Delay(80, ct);
        }
        Console.WriteLine("  Done.");
    }

    // -------------------------------------------------------------------------
    // 3. Linked CancellationTokenSource — combine user cancel + timeout
    // -------------------------------------------------------------------------
    private static async Task LinkedTokenSourceAsync()
    {
        Console.WriteLine("--- Linked token source (user cancel + timeout) ---");
        using var userCts    = new CancellationTokenSource();
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
        using var linked     = CancellationTokenSource.CreateLinkedTokenSource(userCts.Token, timeoutCts.Token);

        try
        {
            // Timeout fires first — linked token cancels automatically
            await Task.Delay(1000, linked.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"  userCts cancelled  : {userCts.IsCancellationRequested}");
            Console.WriteLine($"  timeoutCts cancelled: {timeoutCts.IsCancellationRequested}");
            Console.WriteLine($"  linked cancelled   : {linked.IsCancellationRequested}\n");
        }
    }

    // -------------------------------------------------------------------------
    // 4. Cooperative cancellation with IsCancellationRequested in CPU loops
    //    Use ct.ThrowIfCancellationRequested() as a cheap polling checkpoint.
    // -------------------------------------------------------------------------
    private static async Task CooperativeCheckAsync()
    {
        Console.WriteLine("--- Cooperative cancellation in CPU-bound loop ---");
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        try
        {
            await Task.Run(() => CpuBoundLoop(cts.Token), cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("CPU loop cooperatively cancelled\n");
        }
    }

    private static void CpuBoundLoop(CancellationToken ct)
    {
        var iterations = 0;
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            // Simulate work
            iterations++;
            Thread.SpinWait(10_000);
        }
    }
}
