namespace Collections.Examples;

/// <summary>
/// Demonstrates HashSet<T> — fast membership tests and set operations
/// (union, intersect, except). Use instead of List.Contains() for repeated lookups.
/// </summary>
public static class HashSetExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- HashSet<T> ---");

        var set = new HashSet<int> { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Initial: {string.Join(", ", set)}");

        // Add
        var added = set.Add(6);
        Console.WriteLine($"Add(6): {added}, Set: {string.Join(", ", set)}");

        added = set.Add(3);
        Console.WriteLine($"Add(3): {added} (duplicate, not added)");

        // Contains (fast, O(1))
        Console.WriteLine($"Contains(4): {set.Contains(4)}");
        Console.WriteLine($"Contains(10): {set.Contains(10)}");

        // Remove
        set.Remove(2);
        Console.WriteLine($"After Remove(2): {string.Join(", ", set)}");

        // Set operations
        Console.WriteLine("\nSet Operations:");
        var set1 = new HashSet<int> { 1, 2, 3, 4 };
        var set2 = new HashSet<int> { 3, 4, 5, 6 };

        var union = new HashSet<int>(set1);
        union.UnionWith(set2);
        Console.WriteLine($"Union: {string.Join(", ", union)}");

        var intersect = new HashSet<int>(set1);
        intersect.IntersectWith(set2);
        Console.WriteLine($"Intersect: {string.Join(", ", intersect)}");

        var except = new HashSet<int>(set1);
        except.ExceptWith(set2);
        Console.WriteLine($"Except: {string.Join(", ", except)}");

        var symmetric = new HashSet<int>(set1);
        symmetric.SymmetricExceptWith(set2);
        Console.WriteLine($"Symmetric Except: {string.Join(", ", symmetric)}");

        // IsSubsetOf, IsSupersetOf
        Console.WriteLine($"\nset1.IsSubsetOf(union): {set1.IsSubsetOf(union)}");
        Console.WriteLine($"set1.Overlaps(set2): {set1.Overlaps(set2)}");
    }
}
