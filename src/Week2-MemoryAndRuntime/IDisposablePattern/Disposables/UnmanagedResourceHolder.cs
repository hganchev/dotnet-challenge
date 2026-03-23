namespace IDisposablePattern.Disposables;

/// <summary>
/// Full dispose pattern for an <strong>unsealed</strong> class that wraps
/// a simulated unmanaged resource (represented by a non-zero <see cref="nint"/>).
///
/// Rules applied:
/// - virtual Dispose(bool disposing): managed cleanup when disposing=true,
///   unmanaged cleanup always.
/// - Finalizer (~UnmanagedResourceHolder) as a safety net if Dispose is
///   never called.
/// - GC.SuppressFinalize in Dispose to avoid unnecessary finalizer cost.
/// - Throws ObjectDisposedException from public methods after disposal.
/// </summary>
public class UnmanagedResourceHolder : IDisposable
{
    // Simulate an unmanaged handle (e.g., from a P/Invoke call)
    protected nint _nativeHandle;
    private bool _disposed;

    public string Label { get; }

    public UnmanagedResourceHolder(string label)
    {
        Label = label;
        // Simulate acquiring an unmanaged resource
        _nativeHandle = (nint)Random.Shared.Next(0x1000, 0x9FFF);
        Console.WriteLine($"  [UnmanagedResourceHolder] '{Label}' created. Handle=0x{_nativeHandle:X}");
    }

    /// <summary>Uses the native handle.</summary>
    public string ReadData()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return $"Data from handle 0x{_nativeHandle:X}";
    }

    // ── Public IDisposable entry point ──────────────────────────────────────

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    // ── Finalizer (safety net) ───────────────────────────────────────────────

    ~UnmanagedResourceHolder()
    {
        // disposing=false: do NOT touch managed objects here — they may
        // already be finalized in any order.
        Dispose(disposing: false);
    }

    // ── Protected virtual dispose core ──────────────────────────────────────

    /// <summary>
    /// Core dispose logic. Override in subclasses to release additional
    /// resources; always call base.Dispose(disposing).
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;

        if (disposing)
        {
            // Release managed IDisposable fields here (safe to touch them)
            Console.WriteLine($"  [UnmanagedResourceHolder] '{Label}' releasing managed resources.");
        }

        // Always release unmanaged resources
        if (_nativeHandle != nint.Zero)
        {
            Console.WriteLine($"  [UnmanagedResourceHolder] '{Label}' releasing handle 0x{_nativeHandle:X}.");
            _nativeHandle = nint.Zero; // "close" the handle
        }
    }
}
