using System.Linq.Expressions;

namespace LinqInternals.Concepts;

/// <summary>
/// Demonstrates IQueryable<T> and how expression trees are translated to queries
/// (simulating SQL translation like EF Core does).
/// </summary>
public static class IQueryableExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- IQueryable vs IEnumerable ---");

        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // IEnumerable<T> — uses delegates, in-memory
        Console.WriteLine("IEnumerable<T> (in-memory):");
        var enumerableQuery = numbers.AsEnumerable()
            .Where(x => x > 3)
            .Select(x => x * 2);
        Console.WriteLine($"Query type: {enumerableQuery.GetType().Name}");

        // IQueryable<T> — uses expression trees
        Console.WriteLine("\nIQueryable<T> (expression trees):");
        var queryableQuery = numbers.AsQueryable()
            .Where(x => x > 3)
            .Select(x => x * 2);
        Console.WriteLine($"Query type: {queryableQuery.GetType().Name}");
        Console.WriteLine($"Expression: {queryableQuery.Expression}");

        // In a real scenario with EF Core:
        // IQueryable translates the expression tree to SQL
        // SELECT x * 2 FROM numbers WHERE x > 3
        // Then that SQL is sent to the database server

        Console.WriteLine("\nBoth produce the same results:");
        Console.WriteLine($"IEnumerable results: {string.Join(", ", enumerableQuery)}");
        Console.WriteLine($"IQueryable results: {string.Join(", ", queryableQuery)}");
    }
}
