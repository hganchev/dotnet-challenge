namespace VerticalSlices.SharedKernel;

/// <summary>
/// Shared kernel — small, genuinely cross-cutting pieces referenced by every feature slice:
/// the domain entity itself and the storage abstraction. Kept intentionally minimal; anything
/// feature-specific belongs inside that feature's own folder, even if it duplicates a little
/// code across slices.
/// </summary>
public sealed class Order
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; init; }
    public required decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public interface IOrderStore
{
    void Add(Order order);
    Order? Find(Guid id);
}

public sealed class InMemoryOrderStore : IOrderStore
{
    private readonly Dictionary<Guid, Order> _orders = [];

    public void Add(Order order) => _orders[order.Id] = order;

    public Order? Find(Guid id) => _orders.GetValueOrDefault(id);
}
