namespace AsyncAwait.Concepts;

// ---------------------------------------------------------------------------
// What the C# compiler generates for an async method
//
// Source:
//
//   public static async Task<int> ComputeAsync(int x)
//   {
//       await Task.Delay(10);
//       return x * 2;
//   }
//
// The compiler rewrites this into a struct that implements
// IAsyncStateMachine and drives the continuation itself.
// The simplified equivalent looks like the hand-written code below.
// ---------------------------------------------------------------------------

using System.Runtime.CompilerServices;

/// <summary>
/// Explains the compiler-generated async state machine by showing a
/// hand-written rough equivalent of what the C# compiler produces.
/// Run this to confirm the state machine drives the same observable behaviour.
/// </summary>
public static class AsyncStateMachineExplained
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Async State Machine Explained ===\n");

        // 1. Normal async call — compiler generates the state machine
        Console.WriteLine("--- Regular async call ---");
        var result1 = await ComputeAsync(21);
        Console.WriteLine($"Result (async): {result1}\n");

        // 2. Hand-written state machine equivalent
        Console.WriteLine("--- Hand-written state machine equivalent ---");
        var result2 = await new ComputeStateMachineBuilder(21).StartAsync();
        Console.WriteLine($"Result (state machine): {result2}\n");

        // 3. Show that the compiler-generated state machine transitions
        //    via MoveNext() internal calls through labelled states
        Console.WriteLine("--- State transitions trace ---");
        await TracedComputeAsync(5);
        Console.WriteLine();

        // 4. Key takeaways printed as learner notes
        Console.WriteLine("--- Key takeaways ---");
        Console.WriteLine("  • async methods compile to a struct implementing IAsyncStateMachine");
        Console.WriteLine("  • The struct has an int _state field (−1 = not started, 0,1,... = resume points)");
        Console.WriteLine("  • Each 'await' becomes a labelled resume point in MoveNext()");
        Console.WriteLine("  • The awaiter's IsCompleted fast-path skips the async overhead entirely");
        Console.WriteLine("  • When IsCompleted == false, the continuation is posted to the scheduler");
        Console.WriteLine("  • No threads are blocked — the thread returns to the pool between awaits");
    }

    // ---------------------------------------------------------------------------
    // The source method the compiler transforms
    // ---------------------------------------------------------------------------
    public static async Task<int> ComputeAsync(int x)
    {
        // State 0: resume here after first await
        await Task.Delay(10).ConfigureAwait(false);
        var intermediate = x * 2;

        // State 1: resume here after second await
        await Task.Delay(5).ConfigureAwait(false);

        return intermediate;
    }

    // ---------------------------------------------------------------------------
    // Traced version that prints its own state transitions
    // ---------------------------------------------------------------------------
    private static async Task<int> TracedComputeAsync(int x)
    {
        Console.WriteLine("  [State -1] Method starts, state machine allocated");

        var awaiter1 = Task.Delay(10).ConfigureAwait(false).GetAwaiter();
        if (!awaiter1.IsCompleted)
        {
            Console.WriteLine("  [State  0] Awaiting Task.Delay(10) — suspension point");
        }
        await Task.Delay(10).ConfigureAwait(false);
        Console.WriteLine("  [State  0] Resumed after Task.Delay(10)");

        var intermediate = x * 2;

        var awaiter2 = Task.Delay(5).ConfigureAwait(false).GetAwaiter();
        if (!awaiter2.IsCompleted)
        {
            Console.WriteLine("  [State  1] Awaiting Task.Delay(5) — suspension point");
        }
        await Task.Delay(5).ConfigureAwait(false);
        Console.WriteLine("  [State  1] Resumed after Task.Delay(5)");

        Console.WriteLine("  [State -2] Method complete — Task<int> set to result");
        return intermediate;
    }
}

// ---------------------------------------------------------------------------
// Hand-written rough equivalent of the compiler-generated state machine
// (greatly simplified — the real generated code has more plumbing)
// ---------------------------------------------------------------------------

/// <summary>
/// Approximate hand-written version of what the compiler generates for
/// <see cref="AsyncStateMachineExplained.ComputeAsync"/>.
/// </summary>
file sealed class ComputeStateMachineBuilder(int x)
{
    private readonly int _x = x;

    public Task<int> StartAsync()
    {
        var tcs = new TaskCompletionSource<int>();
        RunStateMachine(tcs, state: 0, intermediate: 0);
        return tcs.Task;
    }

    private void RunStateMachine(TaskCompletionSource<int> tcs, int state, int intermediate)
    {
        switch (state)
        {
            case 0:
                // Entering state 0 — schedule continuation after Delay(10)
                Task.Delay(10).ContinueWith(_ =>
                    RunStateMachine(tcs, state: 1, intermediate: _x * 2),
                    TaskContinuationOptions.ExecuteSynchronously);
                break;

            case 1:
                // Entering state 1 — schedule continuation after Delay(5)
                var captured = intermediate;
                Task.Delay(5).ContinueWith(_ =>
                    RunStateMachine(tcs, state: 2, intermediate: captured),
                    TaskContinuationOptions.ExecuteSynchronously);
                break;

            case 2:
                // Terminal state — set result
                tcs.SetResult(intermediate);
                break;
        }
    }
}
