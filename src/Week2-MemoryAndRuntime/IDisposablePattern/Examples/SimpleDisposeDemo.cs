using IDisposablePattern.Disposables;

namespace IDisposablePattern.Examples;

/// <summary>
/// Demonstrates the simple IDisposable implementation for a sealed class
/// that owns only managed resources.
/// </summary>
public static class SimpleDisposeDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Simple IDisposable (Sealed Class) ===");

        // using declaration — Dispose() called at end of enclosing scope
        Console.WriteLine("\n--- using declaration (C# 8+) ---");
        using var holder = new ManagedResourceHolder("SimpleHolder");
        holder.Write("Hello from managed resource.");
        holder.Write("Second line.");
        Console.WriteLine($"  Read: {holder.ReadAll().Trim()}");
        // holder.Dispose() called automatically here when scope exits

        Console.WriteLine("  Scope exited — Dispose() was called above.");

        // Verify double-Dispose is safe (idempotent)
        Console.WriteLine("\n--- Double-Dispose is safe ---");
        var h2 = new ManagedResourceHolder("DoubleDispose");
        h2.Write("test");
        h2.Dispose();
        h2.Dispose(); // second call — should be a no-op
        Console.WriteLine("  Second Dispose() completed without error.");
    }
}
