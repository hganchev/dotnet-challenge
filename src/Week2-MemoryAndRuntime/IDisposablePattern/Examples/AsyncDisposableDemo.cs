using IDisposablePattern.Disposables;

namespace IDisposablePattern.Examples;

/// <summary>
/// Demonstrates <see cref="IAsyncDisposable"/>/<c>DisposeAsync</c> for cleanup
/// that involves async I/O, consumed via <c>await using</c>.
/// </summary>
public static class AsyncDisposableDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("\n=== IAsyncDisposable (await using) ===");

        await using (var holder = new AsyncResourceHolder("AsyncHolder"))
        {
            await holder.WriteAsync("Hello from an async resource.");
            Console.WriteLine("  Wrote data asynchronously.");
        }
        // holder.DisposeAsync() awaited automatically at the end of the block.

        Console.WriteLine("  Scope exited — DisposeAsync() was awaited above.");
    }
}
