namespace LinqInternals.Examples;

/// <summary>
/// Demonstrates providing a custom <see cref="IEqualityComparer{T}"/> /
/// <see cref="IComparer{T}"/> when the default equality/ordering doesn't
/// match domain semantics — e.g. case-insensitive string keys.
/// </summary>
public static class CustomComparerExample
{
    /// <summary>Case-insensitive equality — "Alice" and "ALICE" are the same key.</summary>
    private sealed class CaseInsensitiveComparer : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y) =>
            string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

        public int GetHashCode(string obj) =>
            obj.ToUpperInvariant().GetHashCode();
    }

    /// <summary>Orders strings by length first, then alphabetically as a tiebreaker.</summary>
    private sealed class LengthThenAlphabeticalComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x is null || y is null) return Comparer<string?>.Default.Compare(x, y);

            var lengthComparison = x.Length.CompareTo(y.Length);
            return lengthComparison != 0
                ? lengthComparison
                : string.CompareOrdinal(x, y);
        }
    }

    public static void Run()
    {
        Console.WriteLine("\n--- Custom IEqualityComparer<T> / IComparer<T> ---");

        // Default HashSet<string> is case-sensitive — "Alice" and "ALICE" are distinct.
        var defaultSet = new HashSet<string> { "Alice", "ALICE", "Bob" };
        Console.WriteLine($"Default HashSet (case-sensitive): {string.Join(", ", defaultSet)} (Count={defaultSet.Count})");

        // With a custom comparer, "Alice" and "ALICE" collapse to one entry.
        var caseInsensitiveSet = new HashSet<string>(new CaseInsensitiveComparer()) { "Alice", "ALICE", "Bob" };
        Console.WriteLine($"Case-insensitive HashSet:          {string.Join(", ", caseInsensitiveSet)} (Count={caseInsensitiveSet.Count})");

        // Dictionary lookups also respect the custom comparer.
        var scoresByName = new Dictionary<string, int>(new CaseInsensitiveComparer())
        {
            ["Alice"] = 95
        };
        Console.WriteLine($"scoresByName[\"ALICE\"] (case-insensitive lookup) = {scoresByName["ALICE"]}");

        // Custom IComparer<T>: sort by length, then alphabetically.
        var words = new List<string> { "kiwi", "fig", "banana", "apple", "date" };
        words.Sort(new LengthThenAlphabeticalComparer());
        Console.WriteLine($"\nSorted by length then alphabetically: {string.Join(", ", words)}");

        // Same custom comparer reused directly with LINQ's OrderBy overload.
        var orderedByLinq = words.OrderBy(w => w, new LengthThenAlphabeticalComparer());
        Console.WriteLine($"Same result via LINQ OrderBy(comparer): {string.Join(", ", orderedByLinq)}");
    }
}
