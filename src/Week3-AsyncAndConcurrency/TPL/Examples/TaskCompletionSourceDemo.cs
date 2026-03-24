namespace TPL.Examples;

/// <summary>
/// Demonstrates TaskCompletionSource&lt;T&gt; for bridging callback-based or
/// event-based APIs into the Task/async model.
/// </summary>
public static class TaskCompletionSourceDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== TaskCompletionSource<T> Demo ===\n");

        await BridgeCallbackApiAsync();
        await BridgeEventApiAsync();
        await TimeoutWithTcsAsync();
        await CancellationWithTcsAsync();
    }

    // -------------------------------------------------------------------------
    // 1. Bridge a callback-based API
    //    Scenario: legacy method that accepts a callback rather than returning Task.
    // -------------------------------------------------------------------------
    private static async Task BridgeCallbackApiAsync()
    {
        Console.WriteLine("--- Bridge callback API ---");

        var result = await WrapCallbackAsTaskAsync(42);
        Console.WriteLine($"  Result from callback-wrapped task: {result}\n");
    }

    private static Task<int> WrapCallbackAsTaskAsync(int input)
    {
        var tcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Simulate a legacy API that calls back when done
        LegacyCallbackApi(input, value =>
        {
            tcs.SetResult(value);          // complete the Task successfully
        }, ex =>
        {
            tcs.SetException(ex);          // propagate failure
        });

        return tcs.Task;
    }

    private static void LegacyCallbackApi(int input, Action<int> onSuccess, Action<Exception> onError)
    {
        // Posted to thread pool to simulate async completion
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Thread.Sleep(30);
            if (input >= 0)
                onSuccess(input * 2);
            else
                onError(new ArgumentException("Input must be non-negative"));
        });
    }

    // -------------------------------------------------------------------------
    // 2. Bridge an event-based API (EAP — Event-based Asynchronous Pattern)
    // -------------------------------------------------------------------------
    private static async Task BridgeEventApiAsync()
    {
        Console.WriteLine("--- Bridge event-based API ---");

        var result = await WrapEventAsTaskAsync();
        Console.WriteLine($"  Event-bridged result: {result}\n");
    }

    private static Task<string> WrapEventAsTaskAsync()
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var sensor = new FakeSensor();

        sensor.DataReady += (_, data) => tcs.TrySetResult(data);
        sensor.ErrorOccurred += (_, ex) => tcs.TrySetException(ex);

        sensor.Start();

        return tcs.Task;
    }

    // -------------------------------------------------------------------------
    // 3. TCS-based timeout — race the real task against a delay
    // -------------------------------------------------------------------------
    private static async Task TimeoutWithTcsAsync()
    {
        Console.WriteLine("--- TCS timeout pattern ---");

        var result = await WithTimeoutAsync(SlowOperationAsync(), TimeSpan.FromMilliseconds(80));
        Console.WriteLine($"  Result with timeout: {result ?? "TIMED OUT"}\n");
    }

    private static async Task<string?> WithTimeoutAsync(Task<string> operation, TimeSpan timeout)
    {
        var tcs = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Complete TCS with null after timeout
        _ = Task.Delay(timeout).ContinueWith(_ => tcs.TrySetResult(null));

        // Complete TCS with the real result when ready
        _ = operation.ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully) tcs.TrySetResult(t.Result);
            else if (t.IsFaulted) tcs.TrySetException(t.Exception!);
        });

        return await tcs.Task;
    }

    private static async Task<string> SlowOperationAsync()
    {
        await Task.Delay(200).ConfigureAwait(false);
        return "slow result";
    }

    // -------------------------------------------------------------------------
    // 4. Cancellation with TCS
    // -------------------------------------------------------------------------
    private static async Task CancellationWithTcsAsync()
    {
        Console.WriteLine("--- TCS with cancellation ---");

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        try
        {
            var result = await WaitForSignalAsync(cts.Token);
            Console.WriteLine($"  Signal received: {result}");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("  Wait cancelled (no signal arrived in time)\n");
        }
    }

    private static Task<string> WaitForSignalAsync(CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Register cancellation: cancel the TCS if token fires
        ct.Register(() => tcs.TrySetCanceled(ct));

        // Signal would arrive here in real code; for demo it never arrives
        return tcs.Task;
    }
}

// ---------------------------------------------------------------------------
// Fake event-based sensor to demonstrate EAP bridging
// ---------------------------------------------------------------------------

file sealed class FakeSensor
{
    public event EventHandler<string>? DataReady;
#pragma warning disable CS0067 // event intentionally unused in this demo (error path)
    public event EventHandler<Exception>? ErrorOccurred;
#pragma warning restore CS0067

    public void Start()
    {
        ThreadPool.QueueUserWorkItem(_ =>
        {
            Thread.Sleep(40);
            DataReady?.Invoke(this, "sensor-reading-42");
        });
    }
}
