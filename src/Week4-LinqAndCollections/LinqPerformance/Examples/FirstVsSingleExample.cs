namespace LinqPerformance.Examples;

/// <summary>
/// Demonstrates that FirstOrDefault() stops at first match,
/// while SingleOrDefault() must scan the entire sequence for uniqueness.
/// </summary>
public static class FirstVsSingleExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- FirstOrDefault vs SingleOrDefault ---");

        var numbers = Enumerable.Range(1, 10_000_000);

        // FirstOrDefault — stops early
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var first = numbers.FirstOrDefault(n => n > 5);
        sw.Stop();
        Console.WriteLine($"FirstOrDefault(n > 5):  {sw.Elapsed.TotalMicroseconds:F0} us, Result: {first}");

        // SingleOrDefault — must scan entire sequence
        sw.Restart();
        try
        {
            var single = numbers.SingleOrDefault(n => n == 42);
            sw.Stop();
            Console.WriteLine($"SingleOrDefault(n==42): {sw.Elapsed.TotalMicroseconds:F0} us, Result: {single}");
        }
        catch (InvalidOperationException)
        {
            sw.Stop();
            Console.WriteLine($"SingleOrDefault(n==42): {sw.Elapsed.TotalMicroseconds:F0} us (throws if not unique)");
        }

        Console.WriteLine("\nRule of Thumb:");
        Console.WriteLine("  Use FirstOrDefault() unless uniqueness is guaranteed");
        Console.WriteLine("  Use SingleOrDefault() only when you NEED to validate uniqueness");
    }
}
