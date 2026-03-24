using System.Threading.Channels;

namespace Channels.Examples;

/// <summary>
/// Demonstrates the fundamental Channel&lt;T&gt; API: creation, writing, and reading.
/// </summary>
public static class BasicChannelDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Basic Channel Demo ===\n");

        await UnboundedChannelAsync();
        await BoundedChannelAsync();
        await ReadAllAsyncPatternAsync();
    }

    // ------------------------------------------------------------------ //
    //  1. Unbounded channel — size is unlimited (use sparingly)
    // ------------------------------------------------------------------ //
    private static async Task UnboundedChannelAsync()
    {
        Console.WriteLine("--- Unbounded channel ---");

        // Channel.CreateUnbounded is fine for low-volume or in-process bridging.
        var channel = Channel.CreateUnbounded<int>();

        // Write 5 items synchronously (always succeeds on unbounded).
        for (var i = 1; i <= 5; i++)
            channel.Writer.TryWrite(i);

        channel.Writer.Complete();

        await foreach (var item in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            Console.Write($"  {item}");

        Console.WriteLine("\n  Channel completed.\n");
    }

    // ------------------------------------------------------------------ //
    //  2. Bounded channel — capacitated, applies back-pressure
    // ------------------------------------------------------------------ //
    private static async Task BoundedChannelAsync()
    {
        Console.WriteLine("--- Bounded channel (capacity = 3) ---");

        var options = new BoundedChannelOptions(capacity: 3)
        {
            FullMode = BoundedChannelFullMode.Wait,   // writer awaits when full
            SingleReader = true,
            SingleWriter = true
        };

        var channel = Channel.CreateBounded<string>(options);

        var writer = Task.Run(async () =>
        {
            foreach (var fruit in new[] { "apple", "banana", "cherry", "date", "elderberry" })
            {
                await channel.Writer.WriteAsync(fruit).ConfigureAwait(false);
                Console.WriteLine($"  wrote  → {fruit}");
            }
            channel.Writer.Complete();
        });

        // Slow reader — forces the writer to experience back-pressure.
        var reader = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                await Task.Delay(30).ConfigureAwait(false);   // simulate slow processing
                Console.WriteLine($"  read   ← {item}");
            }
        });

        await Task.WhenAll(writer, reader).ConfigureAwait(false);
        Console.WriteLine("  Done.\n");
    }

    // ------------------------------------------------------------------ //
    //  3. ReadAllAsync pattern vs manual WaitToReadAsync loop
    // ------------------------------------------------------------------ //
    private static async Task ReadAllAsyncPatternAsync()
    {
        Console.WriteLine("--- ReadAllAsync vs manual WaitToReadAsync ---");

        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(10)
        {
            SingleReader = true,
            SingleWriter = true
        });

        for (var i = 1; i <= 5; i++)
            await channel.Writer.WriteAsync(i).ConfigureAwait(false);
        channel.Writer.Complete();

        // ✅ Preferred: ReadAllAsync is cleaner and handles completion automatically.
        Console.Write("  ReadAllAsync  : ");
        await foreach (var n in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            Console.Write($"{n} ");

        Console.WriteLine();
        Console.WriteLine("  (Prefer ReadAllAsync over manual WaitToReadAsync / TryRead loops)\n");
    }
}
