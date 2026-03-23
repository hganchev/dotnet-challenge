using GarbageCollector.Concepts;

namespace GarbageCollector.Examples;

/// <summary>
/// Demonstrates GC generations: how objects start in Gen0 and get promoted
/// to Gen1 and Gen2 as they survive collection cycles.
/// </summary>
public static class GenerationsDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== GC Generations (Gen0 → Gen1 → Gen2) ===");

        // Create an object — it starts in Gen0
        var longLived = new LongLivedObject("CacheEntry");
        Console.WriteLine($"After creation:        Gen {GC.GetGeneration(longLived)} — {longLived}");

        // Collect Gen0 — the object survives and is promoted to Gen1
        GC.Collect(0, GCCollectionMode.Forced);
        Console.WriteLine($"After Gen0 collect:    Gen {GC.GetGeneration(longLived)}");

        // Collect Gen1 — promoted to Gen2
        GC.Collect(1, GCCollectionMode.Forced);
        Console.WriteLine($"After Gen1 collect:    Gen {GC.GetGeneration(longLived)}");

        // Gen2 is the oldest generation — object stays there
        GC.Collect(2, GCCollectionMode.Forced);
        Console.WriteLine($"After Gen2 collect:    Gen {GC.GetGeneration(longLived)}");

        Console.WriteLine();

        // Short-lived objects that go out of scope are collected in Gen0
        Console.WriteLine("--- Short-lived objects (Gen0 collection) ---");
        CreateShortLivedObjects();

        long before = GC.GetTotalMemory(forceFullCollection: false);
        GC.Collect(0, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        long after = GC.GetTotalMemory(forceFullCollection: false);

        Console.WriteLine($"Memory before Gen0 collect: {before:N0} bytes");
        Console.WriteLine($"Memory after  Gen0 collect: {after:N0} bytes");
        Console.WriteLine($"Freed approximately:        {before - after:N0} bytes");

        // Show current generation counts
        Console.WriteLine();
        Console.WriteLine("--- Collection counts ---");
        for (int gen = 0; gen <= GC.MaxGeneration; gen++)
            Console.WriteLine($"  Gen{gen} collections so far: {GC.CollectionCount(gen)}");
    }

    private static void CreateShortLivedObjects()
    {
        const int count = 1_000;
        for (int i = 0; i < count; i++)
        {
            _ = new ShortLivedObject(i);
        }
        Console.WriteLine($"Created {count:N0} short-lived objects (now out of scope).");
    }
}
