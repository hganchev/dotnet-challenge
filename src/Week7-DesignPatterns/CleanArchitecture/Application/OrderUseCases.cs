namespace CleanArchitecture.Application;

using CleanArchitecture.Domain;

/// <summary>
/// Application layer — depends only on Domain. Defines "ports": interfaces that Infrastructure
/// must implement (Dependency Inversion). Never references EF Core, HTTP, or any concrete
/// technology.
/// </summary>
public interface IOrderRepository
{
    void Save(Order order);
    Order? GetById(Guid id);
}

public sealed record CreateOrderCommand(Guid OrderId, string CustomerName, IReadOnlyList<(string Product, int Quantity, decimal UnitPrice)> Lines);

public sealed record OrderDto(Guid Id, string CustomerName, decimal Total);

/// <summary>Use-case class — one class per use-case. Orchestrates domain logic and persists
/// via the repository port. Contains no infrastructure or presentation concerns.</summary>
public sealed class CreateOrderUseCase(IOrderRepository repository)
{
    public OrderDto Execute(CreateOrderCommand command)
    {
        var order = new Order(command.OrderId, command.CustomerName);
        foreach (var (product, quantity, unitPrice) in command.Lines)
            order.AddLine(product, quantity, unitPrice);

        repository.Save(order);

        return new OrderDto(order.Id, order.CustomerName, order.CalculateTotal());
    }
}

/// <summary>Second use-case — demonstrates a query-style use-case reading through the same port.</summary>
public sealed class GetOrderUseCase(IOrderRepository repository)
{
    public OrderDto? Execute(Guid orderId)
    {
        var order = repository.GetById(orderId);
        return order is null ? null : new OrderDto(order.Id, order.CustomerName, order.CalculateTotal());
    }
}
