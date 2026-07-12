using Microsoft.EntityFrameworkCore;
using QueryOptimization.Data;

namespace QueryOptimization.Concepts;

/// <summary>
/// Raw SQL (<c>FromSqlInterpolated</c>) for queries LINQ can't express well,
/// and bulk operations (<c>ExecuteUpdateAsync</c>/<c>ExecuteDeleteAsync</c>)
/// that modify rows directly in the database without materializing entities.
/// </summary>
public static class RawSqlAndBulkOpsDemo
{
    public static async Task RunAsync(DbContextOptions<ShopDbContext> options)
    {
        Console.WriteLine("--- Raw SQL & Bulk Operations ---");

        await using var context = new ShopDbContext(options);

        var minPrice = 5m;
        var cheapItems = await context.OrderItems
            .FromSqlInterpolated($"SELECT * FROM OrderItems WHERE Price >= {minPrice}")
            .ToListAsync();
        Console.WriteLine($"FromSqlInterpolated: {cheapItems.Count} item(s) with Price >= {minPrice} (parameterized, SQL-injection safe).");

        var updatedRows = await context.OrderItems
            .Where(i => i.ProductName == "Notebook")
            .ExecuteUpdateAsync(setters => setters.SetProperty(i => i.Price, i => i.Price + 1m));
        Console.WriteLine($"ExecuteUpdateAsync: {updatedRows} row(s) updated directly in the database, no entities loaded.");

        var deletedRows = await context.OrderItems
            .Where(i => i.Quantity == 0)
            .ExecuteDeleteAsync();
        Console.WriteLine($"ExecuteDeleteAsync: {deletedRows} row(s) deleted directly, bypassing change tracking.");

        Console.WriteLine();
    }
}
