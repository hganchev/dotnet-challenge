namespace MessagingAndEventDrivenArchitecture.Outbox;

/// <summary>Simulates persisting a business change and its outbox event together, so the event can
/// never be silently lost if publishing to a broker fails right after commit.</summary>
public class OrderRepository
{
    private readonly List<string> _orders = [];
    private readonly InMemoryOutboxStore _outbox;

    public OrderRepository(InMemoryOutboxStore outbox) => _outbox = outbox;

    public void CreateOrder(string orderId)
    {
        // Simulated single local transaction: in a real DB this would be one SaveChanges()/COMMIT
        // that writes both the "orders" row and the "outbox" row atomically.
        _orders.Add(orderId);
        _outbox.Add(new OutboxMessage("OrderCreated", orderId));
        Console.WriteLine($"  [OrderRepository] order {orderId} saved + outbox message queued (same transaction)");
    }
}
