namespace CleanArchitecture.Infrastructure;

using CleanArchitecture.Application;
using CleanArchitecture.Domain;

/// <summary>
/// Infrastructure layer — depends on Application + Domain and provides concrete adapters for
/// the ports Application defines. This in-memory adapter stands in for what would normally be
/// an EF Core `DbContext`-backed repository; swapping technologies only means writing a new
/// adapter here, Application and Domain never change.
/// </summary>
public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _store = [];

    public void Save(Order order) => _store[order.Id] = order;

    public Order? GetById(Guid id) => _store.GetValueOrDefault(id);
}
