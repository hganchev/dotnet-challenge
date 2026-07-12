namespace LinqInternals.Examples;

/// <summary>
/// Demonstrates streaming operators (Select, Where) that process elements one at a time,
/// without materializing the entire sequence in memory.
/// </summary>
public static class StreamingOperatorsExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Streaming Operators ---");

        var numbers = Enumerable.Range(1, 20);

        Console.WriteLine("Streaming chain: Where(even).Select(square).Where(>100).Take(3)");
        var stream = numbers
            .Where(n =>
            {
                Console.WriteLine($"  Where: checking {n}");
                return n % 2 == 0;
            })
            .Select(n =>
            {
                Console.WriteLine($"    Select: squaring {n}");
                return n * n;
            })
            .Where(n =>
            {
                Console.WriteLine($"      Where: checking {n} > 100");
                return n > 100;
            })
            .Take(3);

        Console.WriteLine("Starting iteration...");
        var results = stream.ToList();
        Console.WriteLine($"Results: {string.Join(", ", results)}");
        Console.WriteLine("Notice: iteration stops early with Take(3) — streaming efficiency!");
    }
}
