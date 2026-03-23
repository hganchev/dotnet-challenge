using IDisposablePattern.Disposables;

namespace IDisposablePattern.Examples;

/// <summary>
/// Demonstrates the full dispose pattern for unsealed (inheritable) classes
/// and correct overriding in a derived class.
/// </summary>
public static class FullDisposePatternDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Full Dispose Pattern (Unsealed + Derived) ===");

        // Base class: explicit Dispose() call via using
        Console.WriteLine("\n--- Base class (UnmanagedResourceHolder) ---");
        using (var unmanaged = new UnmanagedResourceHolder("BaseHolder"))
        {
            Console.WriteLine($"  Data: {unmanaged.ReadData()}");
        } // Dispose(true) → handle released, GC.SuppressFinalize called

        Console.WriteLine();

        // Derived class: overrides Dispose(bool) — chains to base
        Console.WriteLine("--- Derived class (DerivedResourceHolder) ---");
        using (var derived = new DerivedResourceHolder("DerivedHolder"))
        {
            Console.WriteLine($"  Data: {derived.ReadData()}");
        }
        // Dispose order: DerivedResourceHolder.Dispose(true) → base.Dispose(true)
        // Releases: extra stream, then native handle

        Console.WriteLine();

        // Finalizer demo: skip Dispose() — finalizer runs as safety net
        Console.WriteLine("--- Finalizer as safety net (no using) ---");
        Console.WriteLine("  Creating UnmanagedResourceHolder WITHOUT using...");
        CreateWithoutDispose();

        Console.WriteLine("  Forcing GC to trigger finalizer...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("  Finalizer ran — Dispose(false) released the handle.");
    }

    private static void CreateWithoutDispose()
    {
        _ = new UnmanagedResourceHolder("ForgotToDispose");
        // No using — Dispose() never called. Finalizer is the fallback.
    }
}
