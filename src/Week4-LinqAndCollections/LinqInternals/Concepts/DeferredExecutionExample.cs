namespace LinqInternals.Concepts;

/// <summary>
/// Demonstrates that LINQ queries are evaluated lazily — the query chain doesn't execute
/// until you iterate (enumerate) the result.
/// </summary>
public static class DeferredExecutionExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Deferred Execution ---");

        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        Console.WriteLine("Building query (no execution yet)...");
        var query = numbers
            .Where(n =>
            {
                Console.WriteLine($"  Evaluating Where({n})");
                return n % 2 == 0;
            })
            .Select(n =>
            {
                Console.WriteLine($"  Evaluating Select({n})");
                return n * n;
            });

        Console.WriteLine("Query built. Now iterating...");
        foreach (var item in query)
        {
            Console.WriteLine($"Result: {item}");
        }

        Console.WriteLine("\nIterating again (query runs again)...");
        foreach (var item in query)
        {
            Console.WriteLine($"Result: {item}");
        }

        Console.WriteLine("\nWith ToList() materialization...");
        var materialized = query.ToList();
        Console.WriteLine("List created. Iterating...");
        foreach (var item in materialized)
        {
            Console.WriteLine($"Result: {item}");
        }
    }
}
