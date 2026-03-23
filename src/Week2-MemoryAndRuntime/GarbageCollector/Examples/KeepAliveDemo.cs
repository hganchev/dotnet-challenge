using GarbageCollector.Concepts;

namespace GarbageCollector.Examples;

/// <summary>
/// Demonstrates GC.KeepAlive() — tells the JIT/GC that a reference must stay
/// reachable until that point in the method, preventing premature collection.
///
/// Why it matters: the JIT can determine that a local variable is no longer used
/// after a certain IL offset and make the object eligible for collection BEFORE
/// the method returns — even while the method is still executing.
/// </summary>
public static class KeepAliveDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== GC.KeepAlive() — Preventing Premature Collection ===");

        DemoWithoutKeepAlive();
        DemoWithKeepAlive();
        DemoFinalizerRaceCondition();
    }

    // -------------------------------------------------------------------------
    // 1. Without KeepAlive — object may be finalized before we're done with it
    // -------------------------------------------------------------------------
    private static void DemoWithoutKeepAlive()
    {
        Console.WriteLine("\n--- Without GC.KeepAlive ---");

        FinalizableResource.ResetCounter();
        var resource = new FinalizableResource("NoKeepAlive");

        // After this point, 'resource' is never read again in this method.
        // The JIT can legally treat it as unreachable and allow collection.
        Console.WriteLine($"  Created: {resource}");

        // Simulate some work that takes time (e.g., a native call that expects
        // the object to stay alive). The JIT may have already marked 'resource'
        // as collectible here.
        SimulateWork(label: "between creation and GC.Collect");

        // Force collection — 'resource' might already be eligible
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("  Finalizer may have run before we expected it!");
        // No GC.KeepAlive here — object lifetime ends whenever JIT decides
    }

    // -------------------------------------------------------------------------
    // 2. With KeepAlive — explicitly extend the object's lifetime
    // -------------------------------------------------------------------------
    private static void DemoWithKeepAlive()
    {
        Console.WriteLine("\n--- With GC.KeepAlive ---");

        FinalizableResource.ResetCounter();
        var resource = new FinalizableResource("WithKeepAlive");

        Console.WriteLine($"  Created: {resource}");

        SimulateWork(label: "between creation and GC.Collect");

        // Force collection — but 'resource' is still reachable because of the
        // GC.KeepAlive call below (the JIT sees the reference is used at end)
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("  resource is guaranteed alive until this line.");

        // GC.KeepAlive is a no-op at runtime — it is purely a JIT fence that
        // ensures 'resource' is not collected before this call site.
        GC.KeepAlive(resource);
        Console.WriteLine("  GC.KeepAlive(resource) reached — object lifetime is now over.");

        // Now a collect can finalize it
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    // -------------------------------------------------------------------------
    // 3. Realistic pattern: SafeNativeOperation — KeepAlive guards a native call
    // -------------------------------------------------------------------------
    private static void DemoFinalizerRaceCondition()
    {
        Console.WriteLine("\n--- Realistic pattern: guarding a native-style operation ---");
        Console.WriteLine("  (Using FinalizableResource as a stand-in for an unmanaged handle)");

        FinalizableResource.ResetCounter();

        // Imagine 'handle' wraps a native resource. Once created, we pass its
        // internal state to a native call. Without KeepAlive the GC could
        // finalize 'handle' (releasing the native resource) in the middle of
        // the native call — a classic race condition in P/Invoke scenarios.
        var handle = new FinalizableResource("NativeHandle");

        Console.WriteLine($"  Opened handle: {handle}");
        Console.WriteLine("  Beginning native-style operation...");

        // --- native call would use handle.InstanceId here ---
        SimulateNativeOperation(handle.InstanceId);

        // KeepAlive ensures the handle is not finalised during SimulateNativeOperation.
        // Place it immediately AFTER the last use of the handle.
        GC.KeepAlive(handle);

        Console.WriteLine("  Native operation complete, handle safely released.");
    }

    private static void SimulateWork(string label)
    {
        // Allocate enough to potentially trigger GC in the middle of our method
        for (int i = 0; i < 100; i++) _ = new byte[1024];
        Console.WriteLine($"  (work done: {label})");
    }

    private static void SimulateNativeOperation(int handleId)
    {
        // Stand-in for a P/Invoke or unsafe block that uses a native handle
        Console.WriteLine($"  [Native] Operating with handle id={handleId}...");
        for (int i = 0; i < 200; i++) _ = new byte[512]; // allocation pressure
        Console.WriteLine($"  [Native] Done with handle id={handleId}");
    }
}
