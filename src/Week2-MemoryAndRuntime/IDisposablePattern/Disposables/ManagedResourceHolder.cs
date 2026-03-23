namespace IDisposablePattern.Disposables;

/// <summary>
/// Simple IDisposable implementation for a <strong>sealed</strong> class
/// that owns only managed IDisposable fields.
///
/// Rules applied:
/// - No finalizer needed (no unmanaged resources).
/// - No virtual Dispose(bool) needed (sealed — cannot be subclassed).
/// - GC.SuppressFinalize is still called as a defensive convention.
/// - Throws ObjectDisposedException on use after disposal.
/// </summary>
public sealed class ManagedResourceHolder : IDisposable
{
    private readonly StringWriter _writer = new();
    private readonly MemoryStream _stream = new();
    private bool _disposed;

    public string Name { get; }

    public ManagedResourceHolder(string name)
    {
        Name = name;
        Console.WriteLine($"  [ManagedResourceHolder] '{Name}' created.");
    }

    /// <summary>Writes a line to the internal buffer.</summary>
    public void Write(string text)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer.WriteLine(text);
        _stream.Write(System.Text.Encoding.UTF8.GetBytes(text));
    }

    /// <summary>Returns everything written so far.</summary>
    public string ReadAll()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _writer.ToString();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _writer.Dispose();
        _stream.Dispose();

        Console.WriteLine($"  [ManagedResourceHolder] '{Name}' disposed.");

        // Defensive: suppress finalizer even though there isn't one,
        // so any future subclass-like patterns stay safe.
        GC.SuppressFinalize(this);
    }
}
