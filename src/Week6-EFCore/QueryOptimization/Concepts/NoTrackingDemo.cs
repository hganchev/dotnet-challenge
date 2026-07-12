using Microsoft.EntityFrameworkCore;
using QueryOptimization.Data;

namespace QueryOptimization.Concepts;

/// <summary>
/// Compares a tracked query against <c>AsNoTracking()</c> by inspecting how
/// many entities the change tracker snapshots in each case.
/// </summary>
public static class NoTrackingDemo
{
    public static async Task RunAsync(DbContextOptions<ShopDbContext> options)
    {
        Console.WriteLine("--- AsNoTracking() vs Tracked Query ---");

        await using (var context = new ShopDbContext(options))
        {
            var tracked = await context.Customers.Include(c => c.Orders).ToListAsync();
            Console.WriteLine($"Tracked query: {tracked.Count} customers loaded, ChangeTracker.Entries() = {context.ChangeTracker.Entries().Count()}");
        }

        await using (var context = new ShopDbContext(options))
        {
            var untracked = await context.Customers.AsNoTracking().Include(c => c.Orders).ToListAsync();
            Console.WriteLine($"AsNoTracking() query: {untracked.Count} customers loaded, ChangeTracker.Entries() = {context.ChangeTracker.Entries().Count()}");
        }

        Console.WriteLine("Use AsNoTracking() for read-only queries — it skips snapshotting entirely, saving memory and CPU.");
        Console.WriteLine();
    }
}
