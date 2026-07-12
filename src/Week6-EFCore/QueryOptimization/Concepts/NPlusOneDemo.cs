using Microsoft.EntityFrameworkCore;
using QueryOptimization.Data;

namespace QueryOptimization.Concepts;

/// <summary>
/// Demonstrates the N+1 problem — one query for the parent rows plus one
/// query per child row — and two ways to fix it: <c>Include()</c> and a
/// <c>Select()</c> projection.
/// </summary>
public static class NPlusOneDemo
{
    public static async Task RunAsync(DbContextOptions<ShopDbContext> options)
    {
        Console.WriteLine("--- N+1 Problem ---");

        Console.WriteLine("Naive (N+1): one query for Orders, then one query per Order for its Items.");
        await using (var context = new ShopDbContext(options))
        {
            var orders = await context.Orders.ToListAsync();
            foreach (var order in orders)
            {
                // Each access below triggers a separate round trip because Items
                // was never loaded — the classic N+1 pattern.
                var items = await context.OrderItems
                    .Where(i => i.OrderId == order.Id)
                    .ToListAsync();
                Console.WriteLine($"  Order {order.Id}: {items.Count} item(s) (separate query)");
            }
        }

        Console.WriteLine("Fixed with Include(): a single query with a JOIN.");
        await using (var context = new ShopDbContext(options))
        {
            var orders = await context.Orders.Include(o => o.Items).ToListAsync();
            foreach (var order in orders)
            {
                Console.WriteLine($"  Order {order.Id}: {order.Items.Count} item(s) (eager loaded)");
            }
        }

        Console.WriteLine("Fixed with Select() projection: only the columns needed, still one query.");
        await using (var context = new ShopDbContext(options))
        {
            var summaries = await context.Orders
                .Select(o => new { o.Id, ItemCount = o.Items.Count })
                .ToListAsync();
            foreach (var summary in summaries)
            {
                Console.WriteLine($"  Order {summary.Id}: {summary.ItemCount} item(s) (projected)");
            }
        }

        Console.WriteLine();
    }
}
