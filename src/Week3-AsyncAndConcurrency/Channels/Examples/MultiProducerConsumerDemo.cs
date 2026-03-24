using System.Threading.Channels;

namespace Channels.Examples;

/// <summary>
/// Multiple producers and multiple consumers sharing a single bounded channel.
/// Shows how any producer can write and any consumer can read, and how to
/// coordinate completion when there are several writers.
/// </summary>
public static class MultiProducerConsumerDemo
{
    private sealed record Message(int ProducerId, int Seq, string Content);
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Multi-Producer / Multi-Consumer Demo ===\n");

        await MultipleProducersOneConsumerAsync();
        await MultipleProducersMultipleConsumersAsync();
    }

    // ------------------------------------------------------------------ //
    //  1. Multiple producers → single consumer
    // ------------------------------------------------------------------ //
    private static async Task MultipleProducersOneConsumerAsync()
    {
        Console.WriteLine("--- 3 producers → 1 consumer ---");

        // SingleWriter = false because several tasks will write concurrently.
        var channel = Channel.CreateBounded<Message>(new BoundedChannelOptions(8)
        {
            SingleWriter = false,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        const int ProducerCount = 3;
        const int ItemsPerProducer = 3;

        var producers = Enumerable.Range(1, ProducerCount)
            .Select(id => ProduceAsync(channel.Writer, producerId: id, items: ItemsPerProducer))
            .ToArray();

        // ✅ Complete the writer only after ALL producers have finished.
        var completionTask = Task.Run(async () =>
        {
            await Task.WhenAll(producers).ConfigureAwait(false);
            channel.Writer.Complete();
            Console.WriteLine("  [All producers done] channel completed");
        });

        var consumerTask = Task.Run(async () =>
        {
            var count = 0;
            await foreach (var msg in channel.Reader.ReadAllAsync().ConfigureAwait(false))
            {
                Console.WriteLine($"  [Consumer] received  P{msg.ProducerId}:#{msg.Seq} — {msg.Content}");
                count++;
            }
            Console.WriteLine($"  [Consumer] total items received: {count}");
        });

        await Task.WhenAll(completionTask, consumerTask).ConfigureAwait(false);
        Console.WriteLine();
    }

    // ------------------------------------------------------------------ //
    //  2. Multiple producers → multiple consumers
    // ------------------------------------------------------------------ //
    private static async Task MultipleProducersMultipleConsumersAsync()
    {
        Console.WriteLine("--- 3 producers → 3 consumers ---");

        const int ProducerCount = 3;
        const int ConsumerCount = 3;
        const int ItemsPerProducer = 4;

        var channel = Channel.CreateBounded<Message>(new BoundedChannelOptions(6)
        {
            SingleWriter = false,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.Wait
        });

        var producers = Enumerable.Range(1, ProducerCount)
            .Select(id => ProduceAsync(channel.Writer, producerId: id, items: ItemsPerProducer))
            .ToArray();

        var consumers = Enumerable.Range(1, ConsumerCount)
            .Select(id => Task.Run(() => ConsumeAsync(channel.Reader, consumerId: id)))
            .ToArray();

        await Task.WhenAll(producers).ConfigureAwait(false);
        channel.Writer.Complete();

        await Task.WhenAll(consumers).ConfigureAwait(false);
        Console.WriteLine();
    }

    private static async Task ProduceAsync(
        ChannelWriter<Message> writer, int producerId, int items)
    {
        for (var seq = 1; seq <= items; seq++)
        {
            var msg = new Message(producerId, seq, $"data-{producerId}.{seq}");
            await writer.WriteAsync(msg).ConfigureAwait(false);
            await Task.Delay(10 * producerId).ConfigureAwait(false);
        }
    }

    private static async Task ConsumeAsync(ChannelReader<Message> reader, int consumerId)
    {
        await foreach (var msg in reader.ReadAllAsync().ConfigureAwait(false))
        {
            await Task.Delay(15).ConfigureAwait(false);
            Console.WriteLine($"  [C{consumerId}] processed P{msg.ProducerId}:#{msg.Seq}");
        }
    }
}


