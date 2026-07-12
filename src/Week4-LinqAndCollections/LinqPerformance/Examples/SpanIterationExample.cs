using System.Runtime.InteropServices;

namespace LinqPerformance.Examples;

/// <summary>
/// Demonstrates Span<T> iteration via CollectionsMarshal.AsSpan() for
/// zero-copy, highly-optimized loops over List<T> internals.
/// </summary>
public static class SpanIterationExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Span<T> Iteration (Zero-Copy) ---");

        var itemCount = 10_000_000;
        var list = Enumerable.Range(0, itemCount).ToList();

        // Standard foreach
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long sum1 = 0;
        foreach (var item in list)
        {
            sum1 += item;
        }
        sw.Stop();
        Console.WriteLine($"foreach (List<T>):       {sw.ElapsedMilliseconds} ms, Sum: {sum1}");

        // Using Span<T> via CollectionsMarshal
        sw.Restart();
        long sum2 = 0;
        var span = CollectionsMarshal.AsSpan(list);
        foreach (var item in span)
        {
            sum2 += item;
        }
        sw.Stop();
        Console.WriteLine($"Span<T> iteration:       {sw.ElapsedMilliseconds} ms, Sum: {sum2}");

        // For-loop with index (traditional)
        sw.Restart();
        long sum3 = 0;
        for (int i = 0; i < list.Count; i++)
        {
            sum3 += list[i];
        }
        sw.Stop();
        Console.WriteLine($"for-loop with index:     {sw.ElapsedMilliseconds} ms, Sum: {sum3}");

        Console.WriteLine("\nConclusion: Span<T> provides maximum performance");
        Console.WriteLine("Use CollectionsMarshal.AsSpan(list) for hot paths!");
    }
}
