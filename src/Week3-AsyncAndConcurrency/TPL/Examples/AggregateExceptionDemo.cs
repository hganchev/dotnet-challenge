namespace TPL.Examples;

/// <summary>
/// Demonstrates AggregateException: how it is thrown, how await unwraps it,
/// how to inspect all inner exceptions, and how to handle them selectively.
/// </summary>
public static class AggregateExceptionDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== AggregateException Demo ===\n");

        await AwaitUnwrapsFirstExceptionAsync();
        await InspectAllExceptionsAsync();
        await HandleSelectivelyAsync();
        await TaskWaitThrowsAggregateAsync();
    }

    // -------------------------------------------------------------------------
    // 1. await re-throws only the FIRST inner exception
    //    The AggregateException is unwrapped; remaining exceptions are lost
    //    unless you inspect the Task.Exception manually.
    // -------------------------------------------------------------------------
    private static async Task AwaitUnwrapsFirstExceptionAsync()
    {
        Console.WriteLine("--- await unwraps first exception ---");

        var t1 = FailAfterAsync(20, new InvalidOperationException("First"));
        var t2 = FailAfterAsync(10, new ArgumentException("Second"));

        try
        {
            await Task.WhenAll(t1, t2);
        }
        catch (Exception ex)
        {
            // await only re-throws the first — ex is NOT AggregateException here
            Console.WriteLine($"  Caught via await: [{ex.GetType().Name}] {ex.Message}");
            Console.WriteLine($"  NOTE: the second exception is silently dropped by await!\n");
        }
    }

    // -------------------------------------------------------------------------
    // 2. Inspect ALL inner exceptions — do not use await; check Task.Exception
    // -------------------------------------------------------------------------
    private static async Task InspectAllExceptionsAsync()
    {
        Console.WriteLine("--- Inspect all exceptions ---");

        var tasks = new[]
        {
            FailAfterAsync(30, new InvalidOperationException("Error A")),
            FailAfterAsync(10, new ArgumentException("Error B")),
            FailAfterAsync(20, new TimeoutException("Error C")),
        };

        // Do NOT await yet — let all tasks run
        await Task.WhenAll(tasks).ContinueWith(_ => { }); // swallow; inspect manually

        var allErrors = tasks
            .Where(t => t.IsFaulted)
            .SelectMany(t => t.Exception!.InnerExceptions)
            .ToList();

        Console.WriteLine($"  Total exceptions: {allErrors.Count}");
        foreach (var ex in allErrors)
            Console.WriteLine($"    [{ex.GetType().Name}] {ex.Message}");
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 3. Handle exceptions selectively with AggregateException.Handle()
    //    Handle() re-throws a new AggregateException for any unhandled ones.
    // -------------------------------------------------------------------------
    private static async Task HandleSelectivelyAsync()
    {
        Console.WriteLine("--- Handle selectively with AggregateException.Handle() ---");

        var tasks = new[]
        {
            FailAfterAsync(10, new InvalidOperationException("Handled type")),
            FailAfterAsync(10, new ArgumentNullException("param", "Also handled")),
            FailAfterAsync(10, new UnauthorizedAccessException("NOT handled — re-thrown")),
        };

        try
        {
            await Task.WhenAll(tasks).ContinueWith(_ => { }); // let all finish

            // Flatten and call Handle on a fresh AggregateException
            var aggEx = new AggregateException(
                tasks.Where(t => t.IsFaulted)
                     .SelectMany(t => t.Exception!.InnerExceptions));

            aggEx.Handle(ex =>
            {
                // Return true = handled; return false = re-throw
                if (ex is InvalidOperationException or ArgumentNullException)
                {
                    Console.WriteLine($"  Handled: [{ex.GetType().Name}] {ex.Message}");
                    return true;
                }
                return false;
            });
        }
        catch (AggregateException remaining)
        {
            Console.WriteLine($"  Unhandled ({remaining.InnerExceptions.Count}): [{remaining.InnerException!.GetType().Name}] {remaining.InnerException.Message}");
        }
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 4. Task.Wait() and Task.Result throw AggregateException directly
    //    (unlike await which unwraps). Avoid both — use await instead.
    // -------------------------------------------------------------------------
    private static async Task TaskWaitThrowsAggregateAsync()
    {
        Console.WriteLine("--- Task.Wait() throws AggregateException (avoid it) ---");

        var faulted = FailAfterAsync(5, new Exception("inner message"));
        await faulted.ContinueWith(_ => { }); // let it fault

        try
        {
            faulted.Wait(); // BLOCKS + wraps in AggregateException
        }
        catch (AggregateException ae)
        {
            Console.WriteLine($"  Task.Wait() threw AggregateException: {ae.InnerException!.Message}");
            Console.WriteLine("  → Prefer 'await task' over 'task.Wait()' in async code.\n");
        }
    }

    // ---------------------------------------------------------------------------
    // Helper — faults a Task after a delay
    // ---------------------------------------------------------------------------
    private static async Task FailAfterAsync(int delayMs, Exception exception)
    {
        await Task.Delay(delayMs).ConfigureAwait(false);
        throw exception;
    }
}
