using VerticalSlices.Features.CreateOrder;
using VerticalSlices.Features.GetOrderById;
using VerticalSlices.SharedKernel;

// -----------------------------------------------------------------------
// Vertical Slice Architecture Demo
//
// Each feature (Features/CreateOrder, Features/GetOrderById) owns its own request, handler,
// and response in a single folder — sliced by feature, not by technical layer. Only the truly
// cross-cutting pieces (the Order entity + storage abstraction) live in SharedKernel/.
// -----------------------------------------------------------------------

Console.WriteLine("=== Vertical Slice Architecture Demo ===\n");

IOrderStore store = new InMemoryOrderStore();

Console.WriteLine("-- Feature: CreateOrder --");
var createHandler = new CreateOrderHandler(store);
var created = createHandler.Handle(new CreateOrderRequest("Alice", 149.99m));
Console.WriteLine($"  Created order {created.OrderId} for {created.CustomerName} — {created.TotalAmount:C} (created {created.CreatedAt:u})");

Console.WriteLine("\n-- Feature: GetOrderById --");
var getHandler = new GetOrderByIdHandler(store);
var found = getHandler.Handle(new GetOrderByIdRequest(created.OrderId));
Console.WriteLine(found is not null
    ? $"  Found order {found.OrderId} — {found.CustomerName}, {found.TotalAmount:C}"
    : "  Order not found.");

var missing = getHandler.Handle(new GetOrderByIdRequest(Guid.NewGuid()));
Console.WriteLine(missing is null
    ? "  Unknown order id → null (expected)."
    : "  Unexpected result.");
