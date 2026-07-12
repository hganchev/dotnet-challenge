namespace CleanArchitecture.Presentation;

using CleanArchitecture.Application;

/// <summary>
/// Presentation layer — depends only on Application. Stands in for a Minimal API controller
/// or CLI command handler: maps external input (here, plain parameters) to an Application
/// command/query and maps the returned DTO back out. Domain entities never appear here.
/// </summary>
public sealed class OrderController(CreateOrderUseCase createOrderUseCase, GetOrderUseCase getOrderUseCase)
{
    public OrderDto CreateOrder(Guid orderId, string customerName, IReadOnlyList<(string Product, int Quantity, decimal UnitPrice)> lines) =>
        createOrderUseCase.Execute(new CreateOrderCommand(orderId, customerName, lines));

    public OrderDto? GetOrder(Guid orderId) => getOrderUseCase.Execute(orderId);
}
