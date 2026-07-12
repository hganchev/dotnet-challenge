namespace IDisposablePattern.Disposables;

/// <summary>
/// Implements both <see cref="IAsyncDisposable"/> and <see cref="IDisposable"/>.
/// Real async cleanup (e.g. flushing a network stream) belongs in
/// <see cref="DisposeAsync"/>; the synchronous <see cref="Dispose"/> is kept
/// only as a fallback for callers that can't <c>await</c> (e.g. inside a
/// synchronous <c>using</c> block), and blocks on the async path to stay correct.
/// </summary>
public sealed class AsyncResourceHolder : IAsyncDisposable, IDisposable
{
    private readonly MemoryStream _stream = new();
    private bool _disposed;

    public string Name { get; }

    public AsyncResourceHolder(string name)
    {
        Name = name;
        Console.WriteLine($"  [AsyncResourceHolder] '{Name}' created.");
    }

    /// <summary>Simulates an async write to the underlying resource.</summary>
    public async Task WriteAsync(string text)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        await _stream.WriteAsync(bytes);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        // Simulate an async flush that a real network/file stream would need.
        await _stream.FlushAsync();
        Console.WriteLine($"  [AsyncResourceHolder] '{Name}' flushed asynchronously and disposed.");

        await _stream.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Synchronous fallback — blocks on the async cleanup. Prefer
    /// `await using` and <see cref="DisposeAsync"/> whenever possible.
    /// </summary>
    public void Dispose() => DisposeAsync().AsTask().GetAwaiter().GetResult();
}
