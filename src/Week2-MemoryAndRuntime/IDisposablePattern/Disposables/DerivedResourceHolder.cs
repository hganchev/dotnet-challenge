namespace IDisposablePattern.Disposables;

/// <summary>
/// Subclass that owns an additional managed IDisposable field.
/// Demonstrates the correct pattern for overriding Dispose(bool) —
/// release own resources first, then call base.
/// </summary>
public sealed class DerivedResourceHolder : UnmanagedResourceHolder
{
    // An extra managed resource owned by this derived class
    private MemoryStream? _extraStream = new();
    private bool _disposed;

    public DerivedResourceHolder(string label) : base(label)
    {
        Console.WriteLine($"  [DerivedResourceHolder] '{label}' created extra resources.");
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;

        if (disposing)
        {
            // Release OUR managed resources before calling base
            _extraStream?.Dispose();
            _extraStream = null;
            Console.WriteLine($"  [DerivedResourceHolder] '{Label}' released extra stream.");
        }

        // Always call base to release the unmanaged handle
        base.Dispose(disposing);
    }
}
