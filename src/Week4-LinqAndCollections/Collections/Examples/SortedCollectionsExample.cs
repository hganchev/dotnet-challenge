namespace Collections.Examples;

/// <summary>
/// Demonstrates SortedDictionary<K,V> and SortedSet<T> for maintaining sorted order
/// at a cost of O(log n) lookups instead of O(1).
/// </summary>
public static class SortedCollectionsExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- SortedDictionary<K,V> and SortedSet<T> ---");

        // SortedDictionary<K,V>
        Console.WriteLine("SortedDictionary<K, V> (sorted by key):");
        var sortedDict = new SortedDictionary<string, int>
        {
            ["Charlie"] = 85,
            ["Alice"] = 92,
            ["Bob"] = 78,
        };

        Console.WriteLine("Iterating (keys in sorted order):");
        foreach (var kvp in sortedDict)
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
        }

        // SortedSet<T>
        Console.WriteLine("\nSortedSet<T> (sorted membership set):");
        var sortedSet = new SortedSet<int> { 5, 1, 9, 3, 7 };
        Console.WriteLine($"After adding items: {string.Join(", ", sortedSet)}");

        // Range operations
        var range = sortedSet.GetViewBetween(3, 7);
        Console.WriteLine($"GetViewBetween(3, 7): {string.Join(", ", range)}");

        // Reverse iteration
        Console.WriteLine($"Reverse: {string.Join(", ", sortedSet.Reverse())}");

        // Min, Max
        Console.WriteLine($"Min: {sortedSet.Min}, Max: {sortedSet.Max}");

        // Note: O(log n) operations
        Console.WriteLine("\nPerformance note: SortedDictionary/SortedSet use O(log n) lookups");
        Console.WriteLine("Use regular Dictionary/HashSet if you don't need sorted order!");
    }
}
