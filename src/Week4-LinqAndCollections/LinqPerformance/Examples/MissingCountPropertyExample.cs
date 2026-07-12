namespace LinqPerformance.Examples;

/// <summary>
/// Demonstrates the performance difference between using the Count property
/// on ICollection<T> (O(1)) vs the Count() LINQ extension, which only enumerates
/// when the underlying sequence can't provide a cheap count.
/// </summary>
public static class MissingCountPropertyExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Count Property vs Count() Extension ---");

        var itemCount = 1_000_000;
        var list = Enumerable.Range(1, itemCount).ToList();

        // Count property (O(1))
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 100_000; i++)
        {
            var _ = list.Count;
        }
        sw.Stop();
        Console.WriteLine($"list.Count (property):    {sw.Elapsed.TotalMicroseconds:F0} us");

        // Count() extension directly on a List<T> — Enumerable.Count() checks
        // "is ICollection<T>" at runtime and short-circuits to the Count property,
        // so THIS IS ALSO O(1), not a full enumeration!
        sw.Restart();
        for (int i = 0; i < 100_000; i++)
        {
            var _ = list.Count();
        }
        sw.Stop();
        Console.WriteLine($"list.Count() (extension): {sw.Elapsed.TotalMicroseconds:F0} us (still O(1) — ICollection<T> fast path)");

        // IMPORTANT: calling .AsEnumerable() does NOT hide the runtime type — the
        // object is still a List<T>, so Count() still takes the fast path below.
        // To force a REAL enumeration, wrap it in Where(), which can't derive the
        // count without evaluating the predicate on every element.
        Console.WriteLine("\nIn LINQ chains (once wrapped in Where(), the fast path is defeated):");
        var chain = list.Where(x => x >= 0);

        sw.Restart();
        for (int i = 0; i < 100; i++)
        {
            var _ = chain.Count();
        }
        sw.Stop();
        Console.WriteLine($"list.Where(...).Count():  {sw.Elapsed.TotalMicroseconds:F0} us for 100 iterations (genuinely enumerates each time)");

        Console.WriteLine("\nRule: Use the .Count property when available (ICollection<T>)");
        Console.WriteLine("      Use .Any() to check if empty");
        Console.WriteLine("      Use .Count() only when necessary in queries");
    }
}
