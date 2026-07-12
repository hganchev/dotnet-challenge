using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueryOptimization.Concepts;
using QueryOptimization.Data;
using QueryOptimization.Entities;

Console.WriteLine("Week 6 - EF Core Query Optimization");
Console.WriteLine("=====================================\n");

var optionsBuilder = new DbContextOptionsBuilder<ShopDbContext>()
    .UseSqlite("Data Source=shop.db")
    .LogTo(Console.WriteLine, LogLevel.Information)
    .EnableSensitiveDataLogging();

var options = optionsBuilder.Options;

await using (var context = new ShopDbContext(options))
{
    await context.Database.MigrateAsync();
    await SeedAsync(context);
}

await NPlusOneDemo.RunAsync(options);
await NoTrackingDemo.RunAsync(options);
await EagerVsExplicitLoadingDemo.RunAsync(options);
await SplitQueryDemo.RunAsync(options);
await CompiledQueryDemo.RunAsync(options);
await RawSqlAndBulkOpsDemo.RunAsync(options);

static async Task SeedAsync(ShopDbContext context)
{
    if (await context.Customers.AnyAsync())
    {
        return;
    }

    var alice = new Customer
    {
        Name = "Alice Anderson",
        Orders =
        {
            new Order
            {
                PlacedAt = DateTime.UtcNow,
                Items =
                {
                    new OrderItem { ProductName = "Notebook", Price = 4.5m, Quantity = 3 },
                    new OrderItem { ProductName = "Pen", Price = 1.2m, Quantity = 0 }
                }
            }
        }
    };

    var bob = new Customer
    {
        Name = "Bob Baker",
        Orders =
        {
            new Order
            {
                PlacedAt = DateTime.UtcNow,
                Items =
                {
                    new OrderItem { ProductName = "Backpack", Price = 25.0m, Quantity = 1 }
                }
            }
        }
    };

    context.Customers.AddRange(alice, bob);
    await context.SaveChangesAsync();
}

