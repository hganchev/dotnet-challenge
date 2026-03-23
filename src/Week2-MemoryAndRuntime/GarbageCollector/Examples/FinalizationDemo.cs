using GarbageCollector.Concepts;

namespace GarbageCollector.Examples;

/// <summary>
/// Demonstrates how finalizers (~destructor) interact with the GC.
/// Finalizable objects are placed on the finalization queue and survive
/// at least one extra GC cycle before being reclaimed.
/// </summary>
public static class FinalizationDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Finalization & the Finalizer Queue ===");

        FinalizableResource.ResetCounter();

        // Create objects with finalizers — they won't be collected immediately
        Console.WriteLine("Creating finalizable objects...");
        CreateFinalizableObjects();

        Console.WriteLine("\nForcing GC.Collect()...");
        GC.Collect();
        Console.WriteLine("GC.Collect() returned — finalizers may not have run yet.");

        Console.WriteLine("\nCalling GC.WaitForPendingFinalizers()...");
        GC.WaitForPendingFinalizers();
        Console.WriteLine("All pending finalizers have completed.");

        // A second collect is needed to actually reclaim the memory
        // because the first collect only moved them to the f-reachable queue
        Console.WriteLine("\nSecond GC.Collect() to reclaim finalized objects...");
        GC.Collect();

        Console.WriteLine();

        // Compare with a non-finalizable object
        Console.WriteLine("--- Non-finalizable objects are collected immediately ---");
        long before = GC.GetTotalMemory(forceFullCollection: true);
        CreateNonFinalizableObjects();
        long mid = GC.GetTotalMemory(forceFullCollection: false);
        GC.Collect();
        long after = GC.GetTotalMemory(forceFullCollection: false);
        Console.WriteLine($"Memory: before={before:N0}, after alloc={mid:N0}, after collect={after:N0}");
    }

    private static void CreateFinalizableObjects()
    {
        for (int i = 1; i <= 3; i++)
        {
            _ = new FinalizableResource($"Resource-{i}");
        }
    }

    private static void CreateNonFinalizableObjects()
    {
        const int count = 500;
        for (int i = 0; i < count; i++)
        {
            _ = new ShortLivedObject(i);
        }
        Console.WriteLine($"  Created {count} non-finalizable objects.");
    }
}
