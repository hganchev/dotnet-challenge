using System.Buffers;

namespace Collections.Examples;

/// <summary>
/// Demonstrates ArrayPool<T> for reusing large temporary buffers,
/// reducing GC pressure and LOH (Large Object Heap) fragmentation.
/// </summary>
public static class ArrayPoolExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- ArrayPool<T> ---");

        // Use a large-enough buffer and enough iterations that allocation/GC
        // cost is actually measurable — 5 iterations of a 1KB buffer is far too
        // small a workload to show any real difference (hence the earlier 0 ms).
        const int bufferSize = 64 * 1024; // 64 KB — large enough to matter for GC
        const int iterations = 50_000;

        // Without ArrayPool: allocates a new array every time — track Gen0
        // collections, which is the REAL cost ArrayPool avoids (not raw fill speed).
        Console.WriteLine("Allocating fresh arrays:");
        var gen0Before = GC.CollectionCount(0);
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            var buffer = new byte[bufferSize];
            buffer[0] = (byte)(i % 256);
        }
        sw.Stop();
        var gen0After = GC.CollectionCount(0);
        Console.WriteLine($"  {iterations} allocations: {sw.ElapsedMilliseconds} ms, Gen0 collections triggered: {gen0After - gen0Before}");

        // With ArrayPool: reuse pooled buffers — no new heap allocation per iteration
        Console.WriteLine("\nUsing ArrayPool:");
        gen0Before = GC.CollectionCount(0);
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                buffer[0] = (byte)(i % 256);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        sw.Stop();
        gen0After = GC.CollectionCount(0);
        Console.WriteLine($"  {iterations} pooled reuses: {sw.ElapsedMilliseconds} ms, Gen0 collections triggered: {gen0After - gen0Before}");
        Console.WriteLine("\nConclusion: ArrayPool's benefit is FEWER GC collections, not raw speed");

        // Typical pattern for temporary buffers
        Console.WriteLine("\nTypical usage pattern:");
        ProcessData();
    }

    private static void ProcessData()
    {
        const int neededSize = 2048;
        var buffer = ArrayPool<byte>.Shared.Rent(neededSize);
        try
        {
            // Use the buffer
            Console.WriteLine($"Rented buffer of size {buffer.Length}");
            for (int i = 0; i < neededSize; i++)
                buffer[i] = 42;
            Console.WriteLine($"Filled buffer with value 42");
        }
        finally
        {
            // Always return the buffer
            ArrayPool<byte>.Shared.Return(buffer);
            Console.WriteLine("Returned buffer to pool");
        }
    }
}
