namespace LinqInternals.Examples;

/// <summary>
/// Demonstrates aggregator operators (First, Single, Count, Sum, etc.)
/// that force materialization and reduction of the sequence.
/// </summary>
public static class AggregatorsExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Aggregators (First, Single, Count, Sum, etc.) ---");

        var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        // First() / FirstOrDefault()
        Console.WriteLine("First() vs FirstOrDefault():");
        var first = numbers.First(n => n > 5);
        Console.WriteLine($"  First(n => n > 5) = {first}");

        var firstOrDefault = numbers.FirstOrDefault(n => n > 100);
        Console.WriteLine($"  FirstOrDefault(n => n > 100) = {firstOrDefault} (not found, returns 0 for int)");

        // Single() / SingleOrDefault()
        Console.WriteLine("\nSingle() vs SingleOrDefault():");
        var special = new[] { 1, 2, 3, 4, 5 };
        var single = special.SingleOrDefault(n => n == 3);
        Console.WriteLine($"  SingleOrDefault(n == 3) = {single}");

        try
        {
            // This throws because Multiple() doesn't exist; demonstrating Single scans all
            var multiple = numbers.FirstOrDefault(n => n > 5);
            Console.WriteLine($"  FirstOrDefault(n => n > 5) = {multiple} (stops at first match)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }

        // Count()
        Console.WriteLine("\nCount():");
        var count = numbers.Count(n => n % 2 == 0);
        Console.WriteLine($"  Count(even) = {count} (scans entire sequence)");

        // Sum(), Average(), Min(), Max()
        Console.WriteLine("\nAggregations:");
        Console.WriteLine($"  Sum() = {numbers.Sum()}");
        Console.WriteLine($"  Average() = {numbers.Average():F2}");
        Console.WriteLine($"  Min() = {numbers.Min()}");
        Console.WriteLine($"  Max() = {numbers.Max()}");

        // All() / Any()
        Console.WriteLine("\nAll() vs Any():");
        var allEven = numbers.All(n => n % 2 == 0);
        Console.WriteLine($"  All(even) = {allEven}");

        var anyEven = numbers.Any(n => n % 2 == 0);
        Console.WriteLine($"  Any(even) = {anyEven} (short-circuits at first match)");

        // Aggregate()
        Console.WriteLine("\nAggregate() (custom reduction):");
        var concatenated = numbers
            .Select(n => n.ToString())
            .Aggregate((acc, curr) => acc + ", " + curr);
        Console.WriteLine($"  Concatenate: {concatenated}");

        var sumViaAggregate = numbers.Aggregate(0, (acc, n) => acc + n);
        Console.WriteLine($"  Sum via Aggregate: {sumViaAggregate}");
    }
}
