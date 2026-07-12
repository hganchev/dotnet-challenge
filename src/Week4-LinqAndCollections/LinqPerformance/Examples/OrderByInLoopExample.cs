namespace LinqPerformance.Examples;

/// <summary>
/// Demonstrates the performance cost of calling OrderBy inside a loop
/// vs sorting once and reusing the result.
/// </summary>
public static class OrderByInLoopExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Don't Call OrderBy in a Loop ---");

        var data = Enumerable.Range(1, 1000).ToList();
        var iterations = 100;

        // Bad: OrderBy inside loop
        Console.WriteLine($"Sorting {data.Count} items, {iterations} times:");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var sorted = data.OrderBy(x => x).ToList();
            // Do something with sorted...
        }
        sw.Stop();
        Console.WriteLine($"  Inside loop: {sw.ElapsedMilliseconds} ms");

        // Good: Sort once, reuse
        sw.Restart();
        var sortedOnce = data.OrderBy(x => x).ToList();
        for (int i = 0; i < iterations; i++)
        {
            // Use sortedOnce...
            var first = sortedOnce.First();
        }
        sw.Stop();
        Console.WriteLine($"  Sort once:   {sw.ElapsedMilliseconds} ms");

        Console.WriteLine("\nBetter yet: cache outside the loop entirely");
    }
}
