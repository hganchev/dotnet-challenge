using System.Buffers;

namespace GarbageCollector.Examples;

/// <summary>
/// Demonstrates the Large Object Heap (LOH): objects ≥ 85,000 bytes
/// are allocated on the LOH and only collected during Gen2 collections.
/// Shows how ArrayPool&lt;T&gt; avoids repeated LOH allocations.
/// </summary>
public static class LargeObjectHeapDemo
{
    private const int LohThreshold = 85_000;

    public static void Run()
    {
        Console.WriteLine("\n=== Large Object Heap (LOH) & ArrayPool<T> ===");

        // Demonstrate LOH allocation
        Console.WriteLine("--- LOH threshold is 85,000 bytes ---");

        var small = new byte[LohThreshold - 1];
        var large = new byte[LohThreshold];

        Console.WriteLine($"byte[{LohThreshold - 1}] → Gen {GC.GetGeneration(small)} (SOH — Small Object Heap)");
        Console.WriteLine($"byte[{LohThreshold}]  → Gen {GC.GetGeneration(large)} (LOH — collected with Gen2)");

        // LOH allocations cause Gen2 collections, which are expensive
        Console.WriteLine("\n--- Repeated LOH allocations without pooling ---");
        long before = GC.GetTotalMemory(forceFullCollection: true);
        int gen2Before = GC.CollectionCount(2);

        const int iterations = 50;
        for (int i = 0; i < iterations; i++)
        {
            _ = new byte[100_000]; // each one lands on the LOH
        }

        GC.Collect();
        long after = GC.GetTotalMemory(forceFullCollection: true);
        int gen2After = GC.CollectionCount(2);
        Console.WriteLine($"  Allocated {iterations} x 100KB arrays");
        Console.WriteLine($"  Gen2 collections: {gen2Before} → {gen2After} (+{gen2After - gen2Before})");

        // Using ArrayPool<T> to avoid LOH pressure
        Console.WriteLine("\n--- ArrayPool<T>.Shared: rent & return ---");
        gen2Before = GC.CollectionCount(2);
        before = GC.GetTotalMemory(forceFullCollection: true);

        for (int i = 0; i < iterations; i++)
        {
            byte[] rented = ArrayPool<byte>.Shared.Rent(100_000);
            // Use the buffer...
            rented.AsSpan(0, 100_000).Clear(); // good practice: clear before use
            ArrayPool<byte>.Shared.Return(rented);
        }

        after = GC.GetTotalMemory(forceFullCollection: false);
        gen2After = GC.CollectionCount(2);
        Console.WriteLine($"  Rented & returned {iterations} x 100KB arrays (pooled)");
        Console.WriteLine($"  Gen2 collections: {gen2Before} → {gen2After} (+{gen2After - gen2Before})");
        Console.WriteLine($"  Memory delta: {after - before:N0} bytes (pool reuses same buffer)");

        // Important: Rent may return a larger array
        Console.WriteLine("\n--- ArrayPool may return a larger buffer ---");
        byte[] buffer = ArrayPool<byte>.Shared.Rent(minimumLength: 100);
        Console.WriteLine($"  Requested: 100 bytes, Received: {buffer.Length} bytes");
        ArrayPool<byte>.Shared.Return(buffer);

        Console.WriteLine("\n  Key takeaways:");
        Console.WriteLine("  • LOH objects are collected only during Gen2 (expensive)");
        Console.WriteLine("  • ArrayPool<T>.Shared reuses buffers — avoids GC pressure");
        Console.WriteLine("  • Always Return() rented buffers (consider clearArray: true for sensitive data)");
    }
}
