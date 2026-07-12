using Microsoft.EntityFrameworkCore;
using QueryOptimization.Data;

namespace QueryOptimization.Concepts;

/// <summary>
/// <c>AsSplitQuery()</c> issues one SQL query per collection <c>Include</c>
/// instead of a single query with multiple JOINs, avoiding the cartesian
/// explosion (row-count multiplication) that a single-query multi-collection
/// include would otherwise cause.
/// </summary>
public static class SplitQueryDemo
{
    public static async Task RunAsync(DbContextOptions<ShopDbContext> options)
    {
        Console.WriteLine("--- AsSplitQuery() ---");

        await using var context = new ShopDbContext(options);

        var singleQueryCustomers = await context.Customers
            .Include(c => c.Orders)
            .ThenInclude(o => o.Items)
            .ToListAsync();
        Console.WriteLine($"Single query (one JOIN-heavy SQL statement): {singleQueryCustomers.Count} customer(s) loaded.");

        var splitQueryCustomers = await context.Customers
            .AsSplitQuery()
            .Include(c => c.Orders)
            .ThenInclude(o => o.Items)
            .ToListAsync();
        Console.WriteLine($"Split query (one SQL statement per collection Include): {splitQueryCustomers.Count} customer(s) loaded.");
        Console.WriteLine("AsSplitQuery() trades one round trip for avoiding row duplication across joined collections.");

        Console.WriteLine();
    }
}
