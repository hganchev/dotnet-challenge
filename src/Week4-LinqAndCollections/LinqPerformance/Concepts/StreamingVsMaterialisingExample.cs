namespace LinqPerformance.Concepts;

/// <summary>
/// Demonstrates the memory vs speed tradeoff between streaming and materialising.
/// Streaming keeps memory low but may iterate multiple times.
/// Materialising uses more memory upfront but enables fast random access.
/// </summary>
public static class StreamingVsMaterialisingExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Streaming vs Materialising Trade-off ---");

        var data = Enumerable.Range(1, 1000);

        // Streaming
        Console.WriteLine("Streaming (lazy):");
        var stream = data
            .Where(n => n % 2 == 0)
            .Select(n => n * n);
        Console.WriteLine("  Query built but not evaluated yet");
        Console.WriteLine("  Taking first 3 items:");
        foreach (var item in stream.Take(3))
        {
            Console.WriteLine($"    {item}");
        }
        Console.WriteLine("  Only processed ~4 items total");

        // Materialising
        Console.WriteLine("\nMaterialising (eager):");
        var materialized = data
            .Where(n => n % 2 == 0)
            .Select(n => n * n)
            .ToList();
        Console.WriteLine($"  List created with {materialized.Count} items");
        Console.WriteLine("  Taking first 3 items:");
        foreach (var item in materialized.Take(3))
        {
            Console.WriteLine($"    {item}");
        }
        Console.WriteLine("  Processed ALL items to create the list");

        Console.WriteLine("\nTrade-off:");
        Console.WriteLine("  Streaming: Low memory, but may iterate multiple times");
        Console.WriteLine("  Materialising: Higher memory, but supports multiple iterations efficiently");
    }
}
