using IDisposablePattern.Disposables;

namespace IDisposablePattern.Examples;

/// <summary>
/// Demonstrates SafeHandle — the preferred way to wrap native handles.
///
/// Key advantages over raw nint:
/// - CLR guarantees ReleaseHandle() even in low-memory / thread-abort scenarios.
/// - Prevents handle recycling / security vulnerabilities.
/// - Auto-integrates with P/Invoke: no manual ref-counting needed.
/// - IsInvalid / IsClosed properties for guard checks.
/// </summary>
public static class SafeHandleDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== SafeHandle ===");

        // 1. Normal use — explicit Dispose via using
        Console.WriteLine("\n--- Normal use with using ---");
        using (var handle = new FileHandleWrapper())
        {
            Console.WriteLine($"  Handle valid: {!handle.IsInvalid}, closed: {handle.IsClosed}");
            Console.WriteLine($"  Raw value:    0x{handle.RawValue:X}");

            UseHandle(handle);
        } // ReleaseHandle() called here via SafeHandle.Dispose()

        Console.WriteLine();

        // 2. IsClosed after Dispose
        Console.WriteLine("--- IsClosed after Dispose ---");
        var h = new FileHandleWrapper();
        Console.WriteLine($"  Before Dispose: IsClosed={h.IsClosed}, IsInvalid={h.IsInvalid}");
        h.Dispose();
        Console.WriteLine($"  After  Dispose: IsClosed={h.IsClosed}");

        Console.WriteLine();

        // 3. SafeHandle vs raw nint — conceptual comparison
        Console.WriteLine("--- Why not raw nint? ---");
        Console.WriteLine("  nint rawHandle = AcquireHandle();");
        Console.WriteLine("  try { Use(rawHandle); }");
        Console.WriteLine("  finally { ReleaseHandle(rawHandle); }  // easy to forget or mis-order");
        Console.WriteLine();
        Console.WriteLine("  SafeHandle: the CLR owns the lifetime — no try/finally needed.");
        Console.WriteLine("  Even on thread abort or low-memory, ReleaseHandle() is guaranteed.");

        // 4. Finalizer safety: SafeHandle finalizes even without using
        Console.WriteLine("\n--- SafeHandle finalizes without using ---");
        CreateWithoutDispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("  ReleaseHandle() called via finalizer.");
    }

    private static void UseHandle(FileHandleWrapper handle)
    {
        Console.WriteLine($"  [UseHandle] Operating with 0x{handle.RawValue:X} ...");
    }

    private static void CreateWithoutDispose()
    {
        _ = new FileHandleWrapper(); // no using — SafeHandle finalizer handles it
    }
}
