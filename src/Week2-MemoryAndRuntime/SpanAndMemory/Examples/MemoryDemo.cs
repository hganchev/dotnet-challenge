using SpanAndMemory.Concepts;

namespace SpanAndMemory.Examples;

/// <summary>
/// Demonstrates <see cref="Memory{T}"/> — the heap-safe, async-compatible
/// counterpart to <see cref="Span{T}"/>.
///
/// Key differences from Span&lt;T&gt;:
/// - Memory&lt;T&gt; is a regular struct (not a ref struct) — can be stored in fields.
/// - Memory&lt;T&gt; can cross <c>await</c> suspension points.
/// - Call <c>.Span</c> property to get a Span for synchronous manipulation.
/// </summary>
public static class MemoryDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("\n=== Memory<T> — Async-Compatible Slicing ===");

        // 1. Why Span<T> cannot cross await
        Console.WriteLine("\n--- Why Span<T> cannot cross await ---");
        Console.WriteLine("  Span<byte> lives on the stack; await may resume on a");
        Console.WriteLine("  different thread — the original stack frame is gone.");
        Console.WriteLine("  // ERROR: Span<byte> span = ...; await Task.Yield();");
        Console.WriteLine("  Use Memory<T> instead — it points to heap memory.");

        // 2. Memory<T> from an array — can be stored and awaited
        Console.WriteLine("\n--- Memory<T> from an array ---");
        byte[] buffer = new byte[64];
        Memory<byte> memory = buffer;
        Console.WriteLine($"  memory.Length = {memory.Length}");
        Console.WriteLine($"  memory.IsEmpty = {memory.IsEmpty}");

        // 3. Passing Memory<T> across an await point
        Console.WriteLine("\n--- Passing Memory<T> across await ---");
        await FillAndVerifyAsync(memory);

        // 4. Slicing Memory<T> without allocation
        Console.WriteLine("\n--- Slicing Memory<T> ---");
        Memory<byte> firstHalf = memory[..32];
        Memory<byte> secondHalf = memory[32..];
        Console.WriteLine($"  original: {memory.Length} bytes");
        Console.WriteLine($"  [..32]:   {firstHalf.Length} bytes, [0]={firstHalf.Span[0]:X2}");
        Console.WriteLine($"  [32..]:   {secondHalf.Length} bytes, [0]={secondHalf.Span[0]:X2}");

        // 5. Memory<T> stored as a class field
        Console.WriteLine("\n--- Memory<T> stored in a class field ---");
        var processor = new BufferProcessor(128);
        await processor.FillAsync(0xCC);
        Console.WriteLine($"  Processor buffer[0]={processor.PeekFirst():X2}, length={processor.Length}");

        // 6. IMemoryOwner<T> from MemoryPool — pooled, lifecycle-managed
        Console.WriteLine("\n--- IMemoryOwner<T> from MemoryPool<T>.Shared ---");
        using var owner = new MemoryOwnerExample(minimumLength: 128);
        Memory<byte> rented = owner.Memory;
        await FillAndVerifyAsync(rented);
        // owner.Dispose() called here — buffer returned to pool
    }

    private static async Task FillAndVerifyAsync(Memory<byte> data)
    {
        int half = data.Length / 2;

        // Fill first half synchronously via .Span
        data.Span[..half].Fill(0xAA);

        // Yield — simulate async work crossing the await boundary
        await Task.Yield();

        // Fill second half after the yield — Memory<T> is still valid
        data.Span[half..].Fill(0xBB);

        byte first = data.Span[0];
        byte second = data.Span[half];
        Console.WriteLine($"  Filled {data.Length} bytes across await: [0]={first:X2} [{half}]={second:X2} ✓");
    }

    /// <summary>
    /// A class that holds a <see cref="Memory{T}"/> field —
    /// impossible with <see cref="Span{T}"/> since it is a ref struct.
    /// </summary>
    private sealed class BufferProcessor(int size)
    {
        // Span<byte> _span; // compile error: cannot store ref struct in a class field
        private readonly Memory<byte> _buffer = new byte[size];

        public int Length => _buffer.Length;
        public byte PeekFirst() => _buffer.Span[0];

        public async Task FillAsync(byte value)
        {
            await Task.Yield(); // cross async boundary
            _buffer.Span.Fill(value);
        }
    }
}
