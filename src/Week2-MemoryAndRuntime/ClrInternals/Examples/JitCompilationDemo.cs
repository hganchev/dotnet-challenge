using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ClrInternals.Examples;

/// <summary>
/// Demonstrates JIT compilation concepts: tiered compilation, cold vs warm
/// paths, and using <see cref="RuntimeHelpers.PrepareMethod"/> to pre-JIT
/// a method before its first call.
/// </summary>
public static class JitCompilationDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== JIT Compilation & Tiered Compilation ===");

        // 1. Tiered compilation overview
        Console.WriteLine("\n--- Tiered Compilation ---");
        Console.WriteLine("  Tier 0: fast, unoptimised code compiled on first call.");
        Console.WriteLine("  Tier 1: re-JIT with full optimisations after method becomes 'hot'.");
        Console.WriteLine("  Hot threshold: ~30 calls (configurable via DOTNET_TC_QuickJitForLoops).");
        Console.WriteLine("  Startup is faster (Tier 0 compiles quicker); throughput is higher (Tier 1).");

        // 2. First-call vs warm-call timing
        Console.WriteLine("\n--- Cold vs warm call timing ---");
        MeasureFirstAndWarmCall();

        // 3. RuntimeHelpers.PrepareMethod — force JIT before first call
        Console.WriteLine("\n--- RuntimeHelpers.PrepareMethod ---");
        Console.WriteLine("  Useful for latency-sensitive code paths that must not JIT on first use.");

        var method = typeof(JitCompilationDemo)
            .GetMethod(nameof(HotPath), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

        var sw = Stopwatch.StartNew();
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        sw.Stop();
        long prepMicros = sw.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        Console.WriteLine($"  PrepareMethod() (pre-JIT): {prepMicros} µs");

        sw.Restart();
        long result = HotPath(1_000_000);
        sw.Stop();
        Console.WriteLine($"  First call after PrepareMethod: {sw.ElapsedMilliseconds} ms (result={result})");

        // 4. RuntimeHelpers.EnsureSufficientExecutionStack
        Console.WriteLine("\n--- RuntimeHelpers.EnsureSufficientExecutionStack ---");
        Console.WriteLine("  Throws InsufficientExecutionStackException instead of StackOverflowException,");
        Console.WriteLine("  allowing the process to survive deep recursion (graceful handling).");
        SafeRecursive(depth: 0, maxDepth: 500);

        // 5. Environment info
        Console.WriteLine("\n--- Runtime environment ---");
        Console.WriteLine($"  DOTNET_TieredCompilation default: enabled");
        Console.WriteLine($"  Disable with: DOTNET_TieredCompilation=0");
        Console.WriteLine($"  RuntimeInformation: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
    }

    private static void MeasureFirstAndWarmCall()
    {
        const int warmIterations = 1_000_000;

        // First call — may be in Tier 0
        var sw = Stopwatch.StartNew();
        long r1 = ComputeSum(100_000);
        sw.Stop();
        long firstMicros = sw.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        Console.WriteLine($"  First call  (Tier 0?): {firstMicros,6} µs  (result={r1})");

        // Warm up — trigger Tier 1 promotion
        for (int i = 0; i < 50; i++) _ = ComputeSum(100);

        // Warm call — should be Tier 1 optimised
        sw.Restart();
        long total = 0;
        for (int i = 0; i < warmIterations; i++) total += ComputeSum(10);
        sw.Stop();
        long nsPerCall = sw.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency / warmIterations;
        Console.WriteLine($"  Warm calls  (Tier 1?): {nsPerCall,6} ns/call average ({warmIterations:N0} iterations)");
        Console.WriteLine($"  (Note: difference more visible in Release build without debugger)");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long ComputeSum(int n)
    {
        long sum = 0;
        for (int i = 0; i <= n; i++) sum += i;
        return sum;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long HotPath(int iterations)
    {
        long acc = 0;
        for (int i = 0; i < iterations; i++) acc += i % 7;
        return acc;
    }

    private static void SafeRecursive(int depth, int maxDepth)
    {
        try
        {
            RuntimeHelpers.EnsureSufficientExecutionStack();
        }
        catch (InsufficientExecutionStackException)
        {
            Console.WriteLine($"  Stack guard triggered at depth {depth} — graceful stop.");
            return;
        }

        if (depth >= maxDepth)
        {
            Console.WriteLine($"  Reached max depth {depth} safely.");
            return;
        }

        SafeRecursive(depth + 1, maxDepth);
    }
}
