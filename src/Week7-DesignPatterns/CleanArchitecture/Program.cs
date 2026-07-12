using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Presentation;

// -----------------------------------------------------------------------
// Clean Architecture Demo
//
// Dependency rule (must point inward only):
//   Presentation -> Application -> Domain
//   Infrastructure -> Application + Domain
//
// Illegal:
//   Domain -> anything            (Domain has zero dependencies)
//   Application -> Infrastructure (would invert the rule; Application only knows the port)
//   Application -> Presentation   (would leak transport concerns into use-cases)
// -----------------------------------------------------------------------

Console.WriteLine("=== Clean Architecture Demo ===\n");

// Composition root: wire the concrete Infrastructure adapter into the Application ports.
IOrderRepository repository = new InMemoryOrderRepository();
var createOrderUseCase = new CreateOrderUseCase(repository);
var getOrderUseCase = new GetOrderUseCase(repository);
var controller = new OrderController(createOrderUseCase, getOrderUseCase);

var orderId = Guid.NewGuid();
var created = controller.CreateOrder(
    orderId,
    "Alice",
    [
        ("Keyboard", 1, 49.99m),
        ("Mouse", 2, 19.99m)
    ]);

Console.WriteLine($"  Created order {created.Id} for {created.CustomerName} — total {created.Total:C}");

var fetched = controller.GetOrder(orderId);
Console.WriteLine(fetched is not null
    ? $"  Fetched order {fetched.Id} — total {fetched.Total:C}"
    : "  Order not found.");

var missing = controller.GetOrder(Guid.NewGuid());
Console.WriteLine(missing is null
    ? "  Unknown order id → null (expected)."
    : "  Unexpected result.");
