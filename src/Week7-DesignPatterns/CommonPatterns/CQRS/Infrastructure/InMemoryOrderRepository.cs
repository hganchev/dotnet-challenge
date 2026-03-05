using CommonPatterns.CQRS.Domain;

namespace CommonPatterns.CQRS.Infrastructure;

/// <summary>Simple in-memory store – simulates a write DB (command side)
/// and a read DB (query side) backed by the same list for demo purposes.</summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];

    public void Add(Order order) => _orders.Add(order);

    public Order? GetById(Guid id) =>
        _orders.FirstOrDefault(o => o.Id == id);

    public IEnumerable<Order> GetAll() => _orders.AsReadOnly();
}
