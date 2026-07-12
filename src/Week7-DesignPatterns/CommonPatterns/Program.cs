using CommonPatterns.Builder;
using CommonPatterns.CQRS;
using CommonPatterns.CQRS.Commands;
using CommonPatterns.CQRS.Infrastructure;
using CommonPatterns.CQRS.Queries;
using CommonPatterns.Decorator;
using CommonPatterns.FactoryMethod;
using CommonPatterns.Observer;
using CommonPatterns.Repository;
using CommonPatterns.Singleton;
using CommonPatterns.Strategy;

// -----------------------------------------------------------------------
// Singleton — thread-safe Lazy<T>
// -----------------------------------------------------------------------
Console.WriteLine("=== Singleton ===");
var config1 = AppConfiguration.Instance;
var config2 = AppConfiguration.Instance;
Console.WriteLine($"  Same instance? {ReferenceEquals(config1, config2)}");
Console.WriteLine($"  Environment setting: {config1.Settings["Environment"]}");

// -----------------------------------------------------------------------
// Factory Method — caller depends only on INotification
// -----------------------------------------------------------------------
Console.WriteLine("\n=== Factory Method ===");
foreach (var channel in new[] { NotificationChannel.Email, NotificationChannel.Sms, NotificationChannel.Push })
{
    var notification = NotificationFactory.Create(channel);
    Console.WriteLine(notification.Send("alice@example.com", "Your order has shipped!"));
}

// -----------------------------------------------------------------------
// Builder — fluent step-by-step construction
// -----------------------------------------------------------------------
Console.WriteLine("\n=== Builder ===");
var pizza = new PizzaBuilder()
    .WithSize("Large")
    .AddTopping("Mozzarella")
    .AddTopping("Pepperoni")
    .WithExtraCheese()
    .Build();
Console.WriteLine($"  {pizza}");

// -----------------------------------------------------------------------
// Strategy — swap algorithm at runtime
// -----------------------------------------------------------------------
Console.WriteLine("\n=== Strategy ===");
var processor = new PaymentProcessor(new CreditCardPayment("4111111111111234"));
Console.WriteLine(processor.ProcessPayment(59.99m));
processor.Strategy = new PayPalPayment("alice@example.com");
Console.WriteLine(processor.ProcessPayment(19.99m));
processor.Strategy = new CryptoPayment("0xA1B2C3D4E5F6");
Console.WriteLine(processor.ProcessPayment(120.00m));

// -----------------------------------------------------------------------
// Observer — C# events, with explicit unsubscribe to avoid leaks
// -----------------------------------------------------------------------
Console.WriteLine("\n=== Observer ===");
var ticker = new StockTicker("MSFT") { Price = 400m };
var subscriber = new PriceAlertSubscriber("Trader-1");
ticker.PriceChanged += subscriber.OnPriceChanged;
ticker.Price = 410m;
ticker.Price = 395m;
ticker.PriceChanged -= subscriber.OnPriceChanged; // Prevents a memory leak: publisher would otherwise keep subscriber alive.
ticker.Price = 500m; // No output — subscriber already unsubscribed.
Console.WriteLine("  (unsubscribed — the 500 move above produced no alert)");

// -----------------------------------------------------------------------
// Decorator — stack behaviour without touching the wrapped type
// -----------------------------------------------------------------------
Console.WriteLine("\n=== Decorator ===");
IBeverage beverage = new HouseBlend();
beverage = new MilkDecorator(beverage);
beverage = new SugarDecorator(beverage);
beverage = new WhipDecorator(beverage);
Console.WriteLine($"  {beverage.Description} = {beverage.Cost:C}");

// -----------------------------------------------------------------------
// Repository — collection-like abstraction over storage (supports DIP)
// -----------------------------------------------------------------------
Console.WriteLine("\n=== Repository ===");
IRepository<Product> productRepository = new InMemoryProductRepository();
productRepository.Add(new Product { Id = Guid.NewGuid(), Name = "Keyboard", Price = 49.99m });
productRepository.Add(new Product { Id = Guid.NewGuid(), Name = "Mouse", Price = 19.99m });
foreach (var product in productRepository.GetAll())
    Console.WriteLine($"  {product.Name}: {product.Price:C}");

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
