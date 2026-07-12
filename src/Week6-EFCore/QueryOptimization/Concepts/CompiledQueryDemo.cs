using Microsoft.EntityFrameworkCore;
using QueryOptimization.Data;

namespace QueryOptimization.Concepts;

/// <summary>
/// <c>EF.CompileAsyncQuery()</c> caches the query's execution plan the first
/// time it runs, skipping re-translation of the LINQ expression to SQL on
/// every subsequent call — worthwhile for hot, frequently executed queries.
/// </summary>
public static class CompiledQueryDemo
{
    private static readonly Func<ShopDbContext, string, IAsyncEnumerable<string>> GetCustomerNamesStartingWith =
        EF.CompileAsyncQuery((ShopDbContext context, string prefix) =>
            context.Customers
                .Where(c => c.Name.StartsWith(prefix))
                .Select(c => c.Name));

    public static async Task RunAsync(DbContextOptions<ShopDbContext> options)
    {
        Console.WriteLine("--- Compiled Query ---");

        await using var context = new ShopDbContext(options);

        await foreach (var name in GetCustomerNamesStartingWith(context, "A"))
        {
            Console.WriteLine($"  Match: {name}");
        }

        Console.WriteLine("Compiled queries skip LINQ-to-SQL translation on repeat calls — worth it only for hot paths, not one-off queries.");
        Console.WriteLine();
    }
}
