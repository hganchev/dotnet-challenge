using GarbageCollector.Examples;

Console.WriteLine("=== Week 2 — Garbage Collector ===");

// --- GC Generations: Gen0 → Gen1 → Gen2 promotion ---
GenerationsDemo.Run();

// --- Finalization: finalizer queue, GC.WaitForPendingFinalizers ---
FinalizationDemo.Run();

// --- GC.Collect(), memory reporting, GCMemoryInfo ---
GcCollectDemo.Run();

// --- Large Object Heap & ArrayPool<T> ---
LargeObjectHeapDemo.Run();

// --- GC.KeepAlive: preventing premature collection ---
KeepAliveDemo.Run();

