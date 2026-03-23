using System.Buffers;
using System.Diagnostics;

namespace SpanAndMemory.Examples;

/// <summary>
/// Demonstrates <c>stackalloc</c> with <see cref="Span{T}"/>: stack allocation
/// for small, short-lived buffers with a runtime size guard to avoid stack overflow.
/// Shows the ArrayPool fallback pattern for larger sizes.
/// </summary>
public static class StackallocDemo
{
    // Guard: allocate on the stack only when ≤ 256 bytes
    private const int StackAllocThreshold = 256;

    public static void Run()
    {
        Console.WriteLine("\n=== stackalloc + Span<T> ===");

        // 1. Basic stackalloc — buffer lives on the stack
        Console.WriteLine("\n--- Basic stackalloc ---");
        Span<int> stackBuffer = stackalloc int[8];
        stackBuffer.Fill(42);
        Console.WriteLine($"  stackalloc int[8] filled with 42: [{string.Join(", ", stackBuffer.ToArray())}]");
        Console.WriteLine("  Buffer lives on the stack — zero GC pressure.");

        // 2. Runtime-sized stackalloc with safety guard
        Console.WriteLine("\n--- Size guard pattern ---");
        ProcessBuffer(smallSize: 64);
        ProcessBuffer(smallSize: 512); // exceeds threshold — uses ArrayPool

        // 3. Using stackalloc for byte encoding (practical example)
        Console.WriteLine("\n--- Practical: UTF-8 encode without allocation ---");
        EncodeAsciiToUpper("hello world");

        // 4. Timing comparison: stackalloc vs new byte[] vs ArrayPool
        Console.WriteLine("\n--- Allocation cost comparison (100K iterations) ---");
        const int iterations = 100_000;
        const int bufferSize = 64;

        // stackalloc: allocate once OUTSIDE the loop — the same stack slot is reused
        // (stackalloc in a loop would overflow the stack for large iteration counts)
        Span<byte> stackBuf = stackalloc byte[bufferSize];
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            stackBuf[0] = (byte)i; // prevent dead-code elimination
        }
        sw.Stop();
        Console.WriteLine($"  stackalloc byte[64] (reused):  {sw.ElapsedMilliseconds,4} ms");
        Console.WriteLine("  (CA2014: stackalloc in a loop is a stack-overflow risk — allocate once outside)");

        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            byte[] buf = new byte[bufferSize];
            buf[0] = (byte)i;
        }
        sw.Stop();
        Console.WriteLine($"  new byte[64] (heap):           {sw.ElapsedMilliseconds,4} ms");

        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            byte[] rented = ArrayPool<byte>.Shared.Rent(bufferSize);
            rented[0] = (byte)i;
            ArrayPool<byte>.Shared.Return(rented);
        }
        sw.Stop();
        Console.WriteLine($"  ArrayPool.Rent/Return byte[64]: {sw.ElapsedMilliseconds,4} ms");
        Console.WriteLine("  (stackalloc is fastest — no heap involvement at all)");
    }

    private static void ProcessBuffer(int smallSize)
    {
        if (smallSize <= StackAllocThreshold)
        {
            Span<byte> buf = stackalloc byte[smallSize];
            buf.Fill(0xFF);
            Console.WriteLine($"  size={smallSize}: stackalloc ({smallSize} ≤ {StackAllocThreshold} threshold)");
        }
        else
        {
            byte[] rented = ArrayPool<byte>.Shared.Rent(smallSize);
            try
            {
                Span<byte> buf = rented.AsSpan(0, smallSize);
                buf.Fill(0xFF);
                Console.WriteLine($"  size={smallSize}: ArrayPool fallback ({smallSize} > {StackAllocThreshold} threshold)");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private static void EncodeAsciiToUpper(string input)
    {
        // Avoid allocating a new string just to upper-case + encode
        Span<byte> bytes = stackalloc byte[input.Length];
        for (int i = 0; i < input.Length; i++)
            bytes[i] = (byte)char.ToUpperInvariant(input[i]);
        Console.WriteLine($"  Input:  '{input}'");
        Console.WriteLine($"  Bytes:  [{string.Join(", ", bytes.ToArray())}]");
        Console.WriteLine("  No heap allocation for the intermediate byte buffer.");
    }
}
