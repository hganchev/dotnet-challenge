using MessagingAndEventDrivenArchitecture.Outbox;

namespace MessagingAndEventDrivenArchitecture.Examples;

/// <summary>Demonstrates the outbox pattern: a business change and its event are queued together,
/// then a separate dispatcher drains and publishes pending events.</summary>
public static class OutboxDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Outbox Pattern ===");
        var outboxStore = new InMemoryOutboxStore();
        var repository = new OrderRepository(outboxStore);
        var dispatcher = new OutboxDispatcher(outboxStore);

        repository.CreateOrder("ORD-1");
        repository.CreateOrder("ORD-2");
        Console.WriteLine($"Pending outbox messages before dispatch: {outboxStore.GetPending().Count}");

        await dispatcher.DispatchPendingAsync();
        Console.WriteLine($"Pending outbox messages after dispatch: {outboxStore.GetPending().Count}");
    }
}
