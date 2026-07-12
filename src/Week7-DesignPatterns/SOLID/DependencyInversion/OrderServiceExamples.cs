namespace SOLID.DependencyInversion;

/// <summary>Violates DIP: the high-level <see cref="OrderServiceBad"/> depends directly on the
/// low-level, concrete <see cref="SqlOrderRepository"/> via <c>new</c> — impossible to unit test
/// or swap storage without editing this class.</summary>
public sealed class SqlOrderRepository
{
    public void Save(string order) => Console.WriteLine($"  [SQL] Saved order: {order}");
}

public sealed class OrderServiceBad
{
    private readonly SqlOrderRepository _repository = new();

    public void PlaceOrder(string order) => _repository.Save(order);
}

/// <summary>Fix: both the high-level service and the low-level repository depend on an
/// abstraction. The concrete implementation is injected via the constructor.</summary>
public interface IOrderRepository
{
    void Save(string order);
}

public sealed class SqlOrderRepositoryV2 : IOrderRepository
{
    public void Save(string order) => Console.WriteLine($"  [SQL] Saved order: {order}");
}

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<string> _orders = [];

    public void Save(string order)
    {
        _orders.Add(order);
        Console.WriteLine($"  [In-Memory] Saved order: {order} (total stored: {_orders.Count})");
    }
}

/// <summary>High-level module — depends only on <see cref="IOrderRepository"/>, so it can be
/// unit-tested with a fake and its storage swapped without any code change here.</summary>
public sealed class OrderService(IOrderRepository repository)
{
    public void PlaceOrder(string order) => repository.Save(order);
}
