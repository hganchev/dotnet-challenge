namespace GarbageCollector.Examples;

/// <summary>
/// Demonstrates GC.Collect(), GC.GetTotalMemory(), and the effect of
/// forced collections on the managed heap. In production, let the GC
/// manage its own schedule — these APIs are for diagnostics and learning.
/// </summary>
public static class GcCollectDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== GC.Collect() & Memory Reporting ===");

        // Baseline
        long baseline = GC.GetTotalMemory(forceFullCollection: true);
        Console.WriteLine($"Baseline managed heap: {baseline:N0} bytes");

        // Allocate a chunk of objects
        const int objectCount = 10_000;
        var objects = new byte[objectCount][];
        for (int i = 0; i < objectCount; i++)
        {
            objects[i] = new byte[128]; // 128 bytes each
        }

        long afterAlloc = GC.GetTotalMemory(forceFullCollection: false);
        Console.WriteLine($"After allocating {objectCount:N0} x 128-byte arrays: {afterAlloc:N0} bytes");
        Console.WriteLine($"Approximate growth: {afterAlloc - baseline:N0} bytes");

        // Drop references — objects are eligible for collection
        objects = null;

        // Collect Gen0 only
        Console.WriteLine("\n--- Collecting Gen0 only ---");
        GC.Collect(0, GCCollectionMode.Forced);
        long afterGen0 = GC.GetTotalMemory(forceFullCollection: false);
        Console.WriteLine($"After Gen0 collect: {afterGen0:N0} bytes");

        // Full collection
        Console.WriteLine("\n--- Full collection (all generations) ---");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect(); // second pass to reclaim finalized objects
        long afterFull = GC.GetTotalMemory(forceFullCollection: false);
        Console.WriteLine($"After full collect:  {afterFull:N0} bytes");
        Console.WriteLine($"Reclaimed approximately: {afterAlloc - afterFull:N0} bytes");

        // GCCollectionMode options
        Console.WriteLine("\n--- GCCollectionMode options ---");
        Console.WriteLine("  Forced:    GC runs immediately regardless of heuristics");
        Console.WriteLine("  Optimized: GC decides whether a collection is productive");
        Console.WriteLine("  Default:   Equivalent to Forced (legacy compat)");

        // Memory info via GC.GetGCMemoryInfo()
        var info = GC.GetGCMemoryInfo();
        Console.WriteLine($"\nGCMemoryInfo snapshot:");
        Console.WriteLine($"  Heap size:          {info.HeapSizeBytes:N0} bytes");
        Console.WriteLine($"  Fragmented:         {info.FragmentedBytes:N0} bytes");
        Console.WriteLine($"  High memory threshold: {info.HighMemoryLoadThresholdBytes:N0} bytes");
        Console.WriteLine($"  Total available:    {info.TotalAvailableMemoryBytes:N0} bytes");
    }
}
