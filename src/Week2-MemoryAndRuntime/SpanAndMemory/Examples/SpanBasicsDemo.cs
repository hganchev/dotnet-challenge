using SpanAndMemory.Concepts;

namespace SpanAndMemory.Examples;

/// <summary>
/// Covers the fundamentals of <see cref="Span{T}"/>: creation, slicing,
/// in-place modification, iteration, and zero-copy stack operations.
/// </summary>
public static class SpanBasicsDemo
{
    public static void Run()
    {
        Console.WriteLine("\n=== Span<T> Basics ===");

        // 1. Span<T> over an array — no copy, same underlying memory
        Console.WriteLine("\n--- Span<T> from an array ---");
        int[] source = [10, 20, 30, 40, 50];
        Span<int> span = source;

        Console.WriteLine($"  Array:  [{string.Join(", ", source)}]");
        span[2] = 99; // modifies the original array through the span
        Console.WriteLine($"  After span[2]=99: [{string.Join(", ", source)}]");

        // 2. Slicing — no allocation, just a window into the same memory
        Console.WriteLine("\n--- Slicing (no allocation) ---");
        Span<int> middle = span[1..4]; // elements 1, 2, 3
        Console.WriteLine($"  span[1..4] = [{string.Join(", ", middle.ToArray())}]");

        Span<int> first3 = span[..3];
        Span<int> last2 = span[^2..];
        Console.WriteLine($"  First 3: [{string.Join(", ", first3.ToArray())}]");
        Console.WriteLine($"  Last  2: [{string.Join(", ", last2.ToArray())}]");

        // 3. ReadOnlySpan<T> — read-only view, works on strings without allocation
        Console.WriteLine("\n--- ReadOnlySpan<char> from a string ---");
        ReadOnlySpan<char> text = "Hello, World!";
        ReadOnlySpan<char> hello = text[..5];
        ReadOnlySpan<char> world = text[7..12];
        Console.WriteLine($"  Full:  {text}");
        Console.WriteLine($"  [..5]: {hello}");
        Console.WriteLine($"  [7..12]: {world}");
        Console.WriteLine($"  Occurrences of 'l': {SpanHelper.CountOccurrences(text, 'l')}");

        // 4. CopyTo — copies data between spans
        Console.WriteLine("\n--- CopyTo and SequenceEqual ---");
        int[] dest = new int[3];
        first3.CopyTo(dest);
        Console.WriteLine($"  CopyTo dest: [{string.Join(", ", dest)}]");
        Console.WriteLine($"  first3.SequenceEqual(dest): {first3.SequenceEqual(dest)}");

        // 5. SpanHelper operations — sum, reverse, findFirst
        Console.WriteLine("\n--- SpanHelper operations ---");
        int[] numbers = [5, 3, 8, 1, 9, 2];
        Console.WriteLine($"  Numbers: [{string.Join(", ", numbers)}]");
        Console.WriteLine($"  Sum: {SpanHelper.Sum(numbers)}");
        SpanHelper.Reverse<int>(numbers);
        Console.WriteLine($"  Reversed: [{string.Join(", ", numbers)}]");
        Console.WriteLine($"  FindFirst(9): index {SpanHelper.FindFirst<int>(numbers, 9)}");

        // 6. Span is a ref struct — cannot be stored in fields or used across await
        Console.WriteLine("\n--- Span<T> is a ref struct ---");
        Console.WriteLine("  • Cannot be stored as a class/struct field");
        Console.WriteLine("  • Cannot be used as a generic type argument");
        Console.WriteLine("  • Cannot cross async suspension points (await)");
        Console.WriteLine("  → Use Memory<T> when you need any of those.");
    }
}
