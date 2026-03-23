namespace SpanAndMemory.Concepts;

/// <summary>
/// Utility methods that operate on <see cref="Span{T}"/> and
/// <see cref="ReadOnlySpan{T}"/> — illustrating zero-allocation algorithms.
/// All methods avoid heap allocations: no arrays, no LINQ, no boxing.
/// </summary>
public static class SpanHelper
{
    /// <summary>Sums all integers in the span without allocating.</summary>
    public static int Sum(ReadOnlySpan<int> values)
    {
        int total = 0;
        foreach (int v in values) total += v;
        return total;
    }

    /// <summary>Reverses a span in-place.</summary>
    public static void Reverse<T>(Span<T> span)
    {
        int left = 0, right = span.Length - 1;
        while (left < right)
        {
            (span[left], span[right]) = (span[right], span[left]);
            left++;
            right--;
        }
    }

    /// <summary>Counts occurrences of a character without allocating.</summary>
    public static int CountOccurrences(ReadOnlySpan<char> text, char target)
    {
        int count = 0;
        foreach (char c in text)
            if (c == target) count++;
        return count;
    }

    /// <summary>
    /// Trims leading and trailing whitespace from a <see cref="ReadOnlySpan{char}"/>
    /// without allocating a new string.
    /// </summary>
    public static ReadOnlySpan<char> TrimSpan(ReadOnlySpan<char> text) => text.Trim();

    /// <summary>
    /// Returns the index of the first element satisfying the predicate,
    /// or -1 if not found.
    /// </summary>
    public static int FindFirst<T>(ReadOnlySpan<T> span, T target)
        where T : IEquatable<T>
    {
        for (int i = 0; i < span.Length; i++)
            if (span[i].Equals(target)) return i;
        return -1;
    }
}
