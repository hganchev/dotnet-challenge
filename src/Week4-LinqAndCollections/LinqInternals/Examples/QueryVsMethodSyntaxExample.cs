namespace LinqInternals.Examples;

/// <summary>
/// Demonstrates query syntax vs method syntax — both compile to the same delegates.
/// Choose based on readability for your scenario.
/// </summary>
public static class QueryVsMethodSyntaxExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Query Syntax vs Method Syntax ---");

        var people = new[]
        {
            new { Name = "Alice", Age = 30 },
            new { Name = "Bob", Age = 25 },
            new { Name = "Charlie", Age = 35 },
            new { Name = "Diana", Age = 28 },
        };

        // Query syntax
        Console.WriteLine("Query Syntax:");
        var queryResult = from p in people
                          where p.Age > 26
                          select $"{p.Name} ({p.Age})";
        foreach (var item in queryResult)
        {
            Console.WriteLine($"  {item}");
        }

        // Method syntax (equivalent)
        Console.WriteLine("\nMethod Syntax (equivalent):");
        var methodResult = people
            .Where(p => p.Age > 26)
            .Select(p => $"{p.Name} ({p.Age})");
        foreach (var item in methodResult)
        {
            Console.WriteLine($"  {item}");
        }

        // Complex query syntax
        Console.WriteLine("\nComplex Query Syntax:");
        var complexQuery = from p in people
                           where p.Age > 26
                           orderby p.Name
                           select new { p.Name, p.Age, Category = p.Age < 30 ? "Young" : "Mature" };
        foreach (var item in complexQuery)
        {
            Console.WriteLine($"  {item.Name}: {item.Age} ({item.Category})");
        }

        // Equivalent method syntax
        Console.WriteLine("\nEquivalent Method Syntax:");
        var complexMethod = people
            .Where(p => p.Age > 26)
            .OrderBy(p => p.Name)
            .Select(p => new { p.Name, p.Age, Category = p.Age < 30 ? "Young" : "Mature" });
        foreach (var item in complexMethod)
        {
            Console.WriteLine($"  {item.Name}: {item.Age} ({item.Category})");
        }
    }
}
