using System.Threading.Channels;

namespace Channels.Examples;

/// <summary>
/// Demonstrates back-pressure in a bounded channel.
/// When the channel is full the producer's <c>WriteAsync</c> awaits, naturally
/// throttling output to the speed of the consumer — no extra rate-limiting code needed.
/// </summary>
public static class BackPressureDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Back-Pressure Demo ===\n");

        await BoundedWaitModeAsync();
        await DropAndDropOldestModeAsync();
    }

    // ------------------------------------------------------------------ //
    //  1. FullMode.Wait — producer blocks when channel is at capacity
    // ------------------------------------------------------------------ //
    private static async Task BoundedWaitModeAsync()
    {
        Console.WriteLine("--- BoundedChannelFullMode.Wait (producer awaits when full) ---");
        Console.WriteLine("    Channel capacity: 3  |  Producer delay: 10 ms  |  Consumer delay: 50 ms");
        Console.WriteLine();

        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(capacity: 3)
        {
            FullMode    = BoundedChannelFullMode.Wait,
            SingleWriter = true,
            SingleReader = true
        });

        var produced = new List<(int item, long ms)>();
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var producerTask = Task.Run(async () =>
        {
            for (var i = 1; i <= 8; i++)
            {
                await channel.Writer.WriteAsync(i).ConfigureAwait(false);
                produced.Add((i, sw.ElapsedMilliseconds));
                Console.WriteLine($"  [P] wrote    {i,2}  at {sw.ElapsedMilliseconds,4} ms");
            }
            channel.Writer.Complete();
        });

        var consumerTask = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                await Task.Delay(50).ConfigureAwait(false);   // slow consumer
                Console.WriteLine($"  [C] consumed {item,2}  at {sw.ElapsedMilliseconds,4} ms");
            }
        });

        await Task.WhenAll(producerTask, consumerTask).ConfigureAwait(false);

        Console.WriteLine();
        Console.WriteLine("  Notice: writes 4-8 are delayed until the consumer reads,");
        Console.WriteLine("  proving that the channel applies back-pressure automatically.\n");
    }

    // ------------------------------------------------------------------ //
    //  2. FullMode.DropOldest — newest always wins, oldest item is evicted
    // ------------------------------------------------------------------ //
    private static async Task DropAndDropOldestModeAsync()
    {
        Console.WriteLine("--- BoundedChannelFullMode.DropOldest (oldest item evicted when full) ---");

        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(capacity: 2)
        {
            FullMode    = BoundedChannelFullMode.DropOldest,
            SingleWriter = true,
            SingleReader = true
        });

        // Write 5 items rapidly without waiting.
        for (var i = 1; i <= 5; i++)
        {
            var accepted = channel.Writer.TryWrite($"item-{i}");
            Console.WriteLine($"  [P] TryWrite item-{i}: {(accepted ? "written" : "dropped oldest to fit")}");
        }
        channel.Writer.Complete();

        Console.Write("  [C] channel contents: ");
        await foreach (var item in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            Console.Write($"{item} ");

        Console.WriteLine();
        Console.WriteLine("  (Only the 2 most-recently written items survive)\n");
    }
}
