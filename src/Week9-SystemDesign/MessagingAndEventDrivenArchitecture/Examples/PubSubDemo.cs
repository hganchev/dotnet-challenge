using System.Threading.Channels;
using MessagingAndEventDrivenArchitecture.PubSub;

namespace MessagingAndEventDrivenArchitecture.Examples;

/// <summary>Demonstrates decoupled producers/consumers: two independent subscribers each receive
/// every published event.</summary>
public static class PubSubDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Pub/Sub via System.Threading.Channels ===");
        var bus = new EventBus();
        var subscriberA = bus.Subscribe();
        var subscriberB = bus.Subscribe();

        var consumerA = ConsumeAsync("Subscriber-A", subscriberA);
        var consumerB = ConsumeAsync("Subscriber-B", subscriberB);

        await bus.PublishAsync(Event.Create("OrderPlaced", "Order #1001"));
        await bus.PublishAsync(Event.Create("OrderShipped", "Order #1001"));
        bus.CompleteAll();

        await Task.WhenAll(consumerA, consumerB);
    }

    private static async Task ConsumeAsync(string subscriberName, ChannelReader<Event> reader)
    {
        await foreach (var evt in reader.ReadAllAsync())
        {
            Console.WriteLine($"  [{subscriberName}] received {evt.Type}: {evt.Payload}");
        }
    }
}
