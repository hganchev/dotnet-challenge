namespace Collections.Concepts;

/// <summary>
/// Demonstrates the performance difference between List<T>.Contains() (O(n))
/// and HashSet<T> membership test (O(1) average).
/// </summary>
public static class HashSetVsListExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- HashSet vs List for Membership Tests ---");

        var itemCount = 200_000;
        var searchCount = 5_000;

        var data = Enumerable.Range(1, itemCount).ToList();
        var list = data;
        var hashSet = data.ToHashSet();

        // IMPORTANT: search items must NOT be clustered near the start of the list,
        // or List<T>.Contains() finds them almost immediately and the O(n) cost
        // never shows up. Use items near the END of the list to force List.Contains()
        // into a near-full linear scan every time (its realistic worst case).
        var searchItems = Enumerable.Range(itemCount - searchCount + 1, searchCount).ToList();

        // List.Contains() — O(n) per lookup, worst case (items near the end)
        Console.WriteLine($"\nSearching {searchCount} items (positioned near the END of the list) in collection of {itemCount}:");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var listMatches = 0;
        foreach (var item in searchItems)
        {
            if (list.Contains(item))
                listMatches++;
        }
        sw.Stop();
        Console.WriteLine($"List.Contains():  {sw.Elapsed.TotalMicroseconds:F0} us, Matches: {listMatches}");

        // HashSet.Contains() — O(1) avg
        sw.Restart();
        var hashMatches = 0;
        foreach (var item in searchItems)
        {
            if (hashSet.Contains(item))
                hashMatches++;
        }
        sw.Stop();
        Console.WriteLine($"HashSet.Contains(): {sw.Elapsed.TotalMicroseconds:F0} us, Matches: {hashMatches}");

        Console.WriteLine("\nConclusion: HashSet is significantly faster for membership tests!");
        Console.WriteLine("For repeated Contains() checks, ALWAYS use HashSet<T>, not List<T>");
    }
}
