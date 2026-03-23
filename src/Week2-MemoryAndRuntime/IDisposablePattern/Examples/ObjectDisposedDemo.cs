using IDisposablePattern.Disposables;

namespace IDisposablePattern.Examples;

/// <summary>
/// Demonstrates ObjectDisposedException — what happens when you call
/// methods on an already-disposed object, and how to throw it correctly.
/// </summary>
public static class ObjectDisposedDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== ObjectDisposedException ===");

        // 1. Reading after disposal
        Console.WriteLine("\n--- Calling a method after Dispose() ---");
        var holder = new ManagedResourceHolder("ThrowsWhenDisposed");
        holder.Write("some data");
        holder.Dispose();

        try
        {
            holder.Write("should throw"); // ObjectDisposedException here
        }
        catch (ObjectDisposedException ex)
        {
            Console.WriteLine($"  Caught: {ex.GetType().Name}");
            Console.WriteLine($"  Object: {ex.ObjectName}");
            Console.WriteLine($"  Message: {ex.Message}");
        }

        Console.WriteLine();

        // 2. Modern throw helper: ObjectDisposedException.ThrowIf
        Console.WriteLine("--- ObjectDisposedException.ThrowIf (modern helper) ---");
        Console.WriteLine("  Usage in the implementation:");
        Console.WriteLine("    ObjectDisposedException.ThrowIf(_disposed, this);");
        Console.WriteLine("  This is equivalent to:");
        Console.WriteLine("    if (_disposed) throw new ObjectDisposedException(GetType().FullName);");
        Console.WriteLine("  The ThrowIf overload is preferred as of .NET 7 — cleaner caller sites.");

        Console.WriteLine();

        // 3. Dispose is idempotent — calling it twice is always safe
        Console.WriteLine("--- Dispose() is idempotent ---");
        var h2 = new ManagedResourceHolder("Idempotent");
        h2.Dispose();
        h2.Dispose(); // must NOT throw — just return on second call
        Console.WriteLine("  Second Dispose() did not throw — correct behaviour.");

        Console.WriteLine();

        // 4. IDisposable and the check pattern
        Console.WriteLine("--- Pattern: check-before-use in public API ---");
        Console.WriteLine("  public void DoWork()");
        Console.WriteLine("  {");
        Console.WriteLine("      ObjectDisposedException.ThrowIf(_disposed, this);");
        Console.WriteLine("      // ... actual work ...");
        Console.WriteLine("  }");
        Console.WriteLine();
        Console.WriteLine("  Every public method that uses managed/unmanaged state");
        Console.WriteLine("  should guard against post-disposal access.");
    }
}
