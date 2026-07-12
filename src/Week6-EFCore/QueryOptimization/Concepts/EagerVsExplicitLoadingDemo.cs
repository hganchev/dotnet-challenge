using Microsoft.EntityFrameworkCore;
using QueryOptimization.Data;

namespace QueryOptimization.Concepts;

/// <summary>
/// Contrasts eager loading (<c>Include</c>/<c>ThenInclude</c>, decided up
/// front) with explicit loading (<c>Entry().Collection().Load()</c>, decided
/// on demand after the parent is already loaded).
/// </summary>
public static class EagerVsExplicitLoadingDemo
{
    public static async Task RunAsync(DbContextOptions<ShopDbContext> options)
    {
        Console.WriteLine("--- Eager vs Explicit Loading ---");

        Console.WriteLine("Eager loading: related data is loaded in the initial query.");
        await using (var context = new ShopDbContext(options))
        {
            var customer = await context.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Items)
                .FirstAsync();
            Console.WriteLine($"  {customer.Name}: {customer.Orders.Count} order(s), loaded up front.");
        }

        Console.WriteLine("Explicit loading: Orders is loaded on demand, after the Customer is already in hand.");
        await using (var context = new ShopDbContext(options))
        {
            var customer = await context.Customers.FirstAsync();
            Console.WriteLine($"  Before Load(): Orders collection has {customer.Orders.Count} entries (unloaded).");

            await context.Entry(customer).Collection(c => c.Orders).LoadAsync();
            Console.WriteLine($"  After Load(): Orders collection has {customer.Orders.Count} entries.");
        }

        Console.WriteLine();
    }
}
