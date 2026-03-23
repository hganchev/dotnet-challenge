using System.Buffers;

namespace SpanAndMemory.Concepts;

/// <summary>
/// Demonstrates correct ownership of a rented buffer from <see cref="MemoryPool{T}"/>.
/// Implements <see cref="IDisposable"/> because it owns an <see cref="IMemoryOwner{T}"/>
/// that must be returned to the pool.
///
/// Unlike <see cref="Span{T}"/>, the <see cref="System.Memory{T}"/> stored here
/// can survive across <c>await</c> points and be stored in class fields.
/// </summary>
public sealed class MemoryOwnerExample : IDisposable
{
    private readonly IMemoryOwner<byte> _owner;
    private bool _disposed;

    /// <param name="minimumLength">Minimum number of bytes needed. The pool may
    /// return a larger buffer — always use <see cref="Memory"/> to stay within bounds.</param>
    public MemoryOwnerExample(int minimumLength)
    {
        _owner = MemoryPool<byte>.Shared.Rent(minimumLength);
        // Rent may return MORE than requested — slice to the exact needed size
        Memory = _owner.Memory[..minimumLength];
        Console.WriteLine($"  [MemoryOwnerExample] Rented {_owner.Memory.Length} bytes (requested {minimumLength})");
    }

    /// <summary>The usable slice of the rented buffer.</summary>
    public Memory<byte> Memory { get; }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _owner.Dispose();
        Console.WriteLine("  [MemoryOwnerExample] Buffer returned to pool.");
    }
}
