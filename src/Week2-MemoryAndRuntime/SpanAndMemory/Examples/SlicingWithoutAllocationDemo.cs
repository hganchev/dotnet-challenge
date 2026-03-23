using System.Runtime.InteropServices;
using SpanAndMemory.Concepts;

namespace SpanAndMemory.Examples;

/// <summary>
/// Demonstrates zero-allocation string and binary slicing using
/// <see cref="ReadOnlySpan{T}"/> and <see cref="MemoryMarshal"/>.
/// </summary>
public static class SlicingWithoutAllocationDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Slicing Without Allocation ===");

        // 1. string.AsSpan() vs Substring — both give the same characters,
        //    but AsSpan() creates no new string allocation
        Console.WriteLine("\n--- string.AsSpan() vs Substring ---");
        const string sentence = "The quick brown fox jumps over the lazy dog";
        ReadOnlySpan<char> span = sentence.AsSpan();

        // Find "fox" and extract the word after it
        int foxIdx = span.IndexOf("fox");
        ReadOnlySpan<char> afterFox = span[(foxIdx + 4)..]; // "jumps over the lazy dog"
        int space = afterFox.IndexOf(' ');
        ReadOnlySpan<char> firstWord = afterFox[..space]; // "jumps" — no allocation

        Console.WriteLine($"  Sentence: \"{sentence}\"");
        Console.WriteLine($"  First word after 'fox': '{firstWord}'  (no string allocation)");
        Console.WriteLine($"  Substring equivalent:   '{sentence.Substring(foxIdx + 4, space)}' (allocates)");

        // 2. ReadOnlySpan<char> for common string operations without allocation
        Console.WriteLine("\n--- Zero-alloc string operations ---");
        ReadOnlySpan<char> csv = "  Alice , Bob , Carol  ";
        Console.WriteLine($"  Raw: '{csv}'");

        int start = 0;
        while (start < csv.Length)
        {
            int comma = csv[start..].IndexOf(',');
            ReadOnlySpan<char> token = comma >= 0
                ? SpanHelper.TrimSpan(csv[start..(start + comma)])
                : SpanHelper.TrimSpan(csv[start..]);

            if (!token.IsEmpty)
                Console.WriteLine($"    Token: '{token}'");

            if (comma < 0) break;
            start += comma + 1;
        }

        // 3. MemoryMarshal.Cast — reinterpret bytes as another primitive type
        Console.WriteLine("\n--- MemoryMarshal.Cast<byte, int> (zero-copy reinterpret) ---");
        // Two little-endian int32 values stored as bytes
        byte[] rawBytes = [0x01, 0x00, 0x00, 0x00,   // int 1
                           0x2A, 0x00, 0x00, 0x00];  // int 42
        ReadOnlySpan<int> ints = MemoryMarshal.Cast<byte, int>(rawBytes);
        Console.WriteLine($"  Bytes: [{string.Join(", ", rawBytes)}]");
        Console.WriteLine($"  As int[]: [{ints[0]}, {ints[1]}]  (no copy, same memory)");

        // 4. MemoryMarshal.GetReference — get a ref to the first element
        Console.WriteLine("\n--- MemoryMarshal.GetReference ---");
        ReadOnlySpan<char> word = "Copilot";
        ref readonly char firstChar = ref MemoryMarshal.GetReference(word);
        Console.WriteLine($"  First char of '{word}' via ref: '{firstChar}'");

        // 5. Overhead comparison
        Console.WriteLine("\n--- Why it matters ---");
        Console.WriteLine("  Each Substring() allocates a new string on the heap.");
        Console.WriteLine("  In a hot parsing loop (e.g. 100K HTTP headers), that's");
        Console.WriteLine("  100K strings for the GC to collect — avoidable with Span.");
        Console.WriteLine("  ReadOnlySpan<char> is just a (pointer, length) pair: 0 bytes allocated.");
    }
}
