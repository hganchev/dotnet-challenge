namespace LinqPerformance.Examples;

/// <summary>
/// Demonstrates optimized LINQ query patterns:
/// - Streaming with minimal materialisation
/// - Using Any() for early termination
/// - Chaining efficiently
/// </summary>
public static class OptimisedQueryExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Optimised LINQ Patterns ---");

        var numbers = Enumerable.Range(1, 100);

        // Good: Streaming chain, final materialisation only
        Console.WriteLine("Good: Streaming → Take → ToList");
        var result1 = numbers
            .Where(n => n % 2 == 0)      // Streaming filter
            .Select(n => n * n)          // Streaming projection
            .Where(n => n > 100)         // Streaming filter
            .Take(5)                     // Early termination
            .ToList();                   // Materialise only what's needed
        Console.WriteLine($"  Result: {string.Join(", ", result1)}");

        // Less optimal: Multiple materialisation
        Console.WriteLine("\nLess optimal: Multiple ToList() calls");
        var temp1 = numbers.Where(n => n % 2 == 0).ToList();      // Materialise
        var temp2 = temp1.Select(n => n * n).ToList();            // Materialise again
        var temp3 = temp2.Where(n => n > 100).ToList();           // Materialise again
        var result2 = temp3.Take(5).ToList();                      // Materialise again
        Console.WriteLine($"  Result: {string.Join(", ", result2)}");

        // Batch processing example
        Console.WriteLine("\nBatch processing pattern:");
        var data = Enumerable.Range(1, 1000);
        var batches = BatchStream(data, 100).ToList();
        Console.WriteLine($"  Split into {batches.Count} batches of 100 items");
        Console.WriteLine($"  First batch: {string.Join(", ", batches[0].Take(5))}...");
    }

    /// <summary>
    /// Splits a sequence into batches while streaming.
    /// </summary>
    private static IEnumerable<IEnumerable<T>> BatchStream<T>(IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch.ToList();
                batch.Clear();
            }
        }
        if (batch.Count > 0)
            yield return batch;
    }
}
