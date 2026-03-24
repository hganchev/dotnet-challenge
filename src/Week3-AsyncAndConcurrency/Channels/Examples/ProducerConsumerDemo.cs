using System.Threading.Channels;

namespace Channels.Examples;

/// <summary>
/// Single producer / single consumer pattern over a bounded channel.
/// Demonstrates how the producer and consumer run concurrently and how
/// <c>writer.Complete()</c> signals end-of-stream to the consumer.
/// </summary>
public static class ProducerConsumerDemo
{
    private sealed record WorkItem(int Id, string Payload);
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Producer / Consumer Demo ===\n");

        await SingleProducerSingleConsumerAsync();
        await WithCancellationAsync();
    }

    // ------------------------------------------------------------------ //
    //  1. Basic single producer / single consumer
    // ------------------------------------------------------------------ //
    private static async Task SingleProducerSingleConsumerAsync()
    {
        Console.WriteLine("--- Single producer, single consumer ---");

        var channel = Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(4)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        var producerTask = ProduceAsync(channel.Writer, count: 8);
        var consumerTask = ConsumeAsync(channel.Reader);

        await Task.WhenAll(producerTask, consumerTask).ConfigureAwait(false);
        Console.WriteLine("  All items processed.\n");
    }

    private static async Task ProduceAsync(ChannelWriter<WorkItem> writer, int count)
    {
        for (var i = 1; i <= count; i++)
        {
            var item = new WorkItem(Id: i, Payload: $"job-{i}");
            await writer.WriteAsync(item).ConfigureAwait(false);
            Console.WriteLine($"  [Producer] sent     #{item.Id}: {item.Payload}");
            await Task.Delay(20).ConfigureAwait(false);   // simulate work between sends
        }

        // ✅ Always call Complete() so the consumer knows there are no more items.
        writer.Complete();
        Console.WriteLine("  [Producer] done — channel completed");
    }

    private static async Task ConsumeAsync(ChannelReader<WorkItem> reader)
    {
        await foreach (var item in reader.ReadAllAsync().ConfigureAwait(false))
        {
            await Task.Delay(35).ConfigureAwait(false);   // simulate slower processing
            Console.WriteLine($"  [Consumer] processed #{item.Id}: {item.Payload}");
        }
    }

    // ------------------------------------------------------------------ //
    //  2. Cooperative cancellation mid-stream
    // ------------------------------------------------------------------ //
    private static async Task WithCancellationAsync()
    {
        Console.WriteLine("--- Cancellation mid-stream ---");

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(120));

        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(2)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        var producerTask = Task.Run(async () =>
        {
            for (var i = 1; ; i++)
            {
                try
                {
                    await channel.Writer.WriteAsync(i, cts.Token).ConfigureAwait(false);
                    Console.WriteLine($"  [P] sent {i}");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("  [P] cancelled — stopping producer");
                    break;
                }
                await Task.Delay(30).ConfigureAwait(false);
            }
            channel.Writer.TryComplete();
        });

        var consumerTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var n in channel.Reader
                    .ReadAllAsync(cts.Token).ConfigureAwait(false))
                {
                    await Task.Delay(25).ConfigureAwait(false);
                    Console.WriteLine($"  [C] consumed {n}");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("  [C] cancelled");
            }
        });

        await Task.WhenAll(producerTask, consumerTask).ConfigureAwait(false);
        Console.WriteLine("  Cancelled cleanly.\n");
    }
}


