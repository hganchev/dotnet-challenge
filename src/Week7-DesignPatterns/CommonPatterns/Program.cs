using CommonPatterns.CQRS;
using CommonPatterns.CQRS.Commands;
using CommonPatterns.CQRS.Infrastructure;
using CommonPatterns.CQRS.Queries;

// -----------------------------------------------------------------------
// CQRS Demo – no MediatR, pure .NET generics
//
// Structure:
//   ICommand / ICommandHandler<TCommand>   – write side
//   IQuery<TResult> / IQueryHandler<TQuery,TResult> – read side
//   Dispatcher                              – routes to registered handlers
// -----------------------------------------------------------------------

var repository = new InMemoryOrderRepository();

var dispatcher = new Dispatcher()
    .Register(new CreateOrderCommandHandler(repository))
    .Register<GetOrderByIdQuery, OrderResult?>(new GetOrderByIdQueryHandler(repository))
    .Register<GetAllOrdersQuery, IEnumerable<OrderResult>>(new GetAllOrdersQueryHandler(repository));

// --- Write side: send commands ---
Console.WriteLine("=== CQRS Demo ===\n");
Console.WriteLine("-- Commands (write side) --");

var orderId1 = Guid.NewGuid();
var orderId2 = Guid.NewGuid();

await dispatcher.SendAsync(new CreateOrderCommand
{
    OrderId = orderId1,
    CustomerName = "Alice",
    TotalAmount = 149.99m
});

await dispatcher.SendAsync(new CreateOrderCommand
{
    OrderId = orderId2,
    CustomerName = "Bob",
    TotalAmount = 299.00m
});

// --- Read side: issue queries ---
Console.WriteLine("\n-- Queries (read side) --");

var order = await dispatcher.QueryAsync<GetOrderByIdQuery, OrderResult?>(
    new GetOrderByIdQuery { OrderId = orderId1 });

Console.WriteLine(order is not null
    ? $"  [GetOrderByIdQueryHandler] Found » {order.CustomerName} – {order.TotalAmount:C} (created {order.CreatedAt:u})"
    : "  [GetOrderByIdQueryHandler] Order not found.");

var allOrders = await dispatcher.QueryAsync<GetAllOrdersQuery, IEnumerable<OrderResult>>(
    new GetAllOrdersQuery());

Console.WriteLine("  [GetAllOrdersQueryHandler] All orders:");
foreach (var o in allOrders)
    Console.WriteLine($"    • {o.Id} | {o.CustomerName} | {o.TotalAmount:C}");

// --- Unknown order (null result) ---
var missing = await dispatcher.QueryAsync<GetOrderByIdQuery, OrderResult?>(
    new GetOrderByIdQuery { OrderId = Guid.NewGuid() });

Console.WriteLine(missing is null
    ? "\n  [GetOrderByIdQueryHandler] Unknown Id → returned null (expected)."
    : $"\n  Unexpected result: {missing}");
