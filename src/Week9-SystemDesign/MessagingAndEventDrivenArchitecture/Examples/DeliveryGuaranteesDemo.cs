using MessagingAndEventDrivenArchitecture.DeliveryGuarantees;
using MessagingAndEventDrivenArchitecture.PubSub;

namespace MessagingAndEventDrivenArchitecture.Examples;

/// <summary>Demonstrates why "exactly-once" delivery is effectively achieved via an idempotent
/// consumer, not by the transport: an at-least-once transport can redeliver the same message, but
/// the consumer only ever acts on it once.</summary>
public static class DeliveryGuaranteesDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Delivery Guarantees: At-Least-Once + Idempotent Consumer ===");
        var evt = Event.Create("PaymentProcessed", "Payment #55");
        var consumer = new IdempotentConsumer();

        // Simulate the transport redelivering the exact same message 3 times
        // (e.g. after a broker ack timeout or a consumer crash-and-restart).
        consumer.Consume(evt);
        consumer.Consume(evt);
        consumer.Consume(evt);

        Console.WriteLine($"Total distinct events processed: {consumer.ProcessedCount} (expected 1 despite 3 deliveries)");
    }
}
