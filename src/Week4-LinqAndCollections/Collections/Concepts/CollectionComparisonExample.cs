namespace Collections.Concepts;

/// <summary>
/// Shows the O(n) vs O(1) complexity tradeoffs for different collections
/// and when to use each one.
/// </summary>
public static class CollectionComparisonExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Collection Complexity Comparison ---");

        Console.WriteLine("Collection Performance Summary:");
        Console.WriteLine(@"
Collection          | Get/Search | Insert | Remove | Use Case
--------------------|------------|--------|--------|-----------------------------------
List<T>             | O(1)       | O(n)   | O(n)   | Ordered, index access
Dictionary<K,V>     | O(1) avg   | O(1)   | O(1)   | Fast key lookups
HashSet<T>          | O(1) avg   | O(1)   | O(1)   | Membership, set ops
SortedDict<K,V>     | O(log n)   | O(log n) | O(log n) | Sorted, ordered iteration
LinkedList<T>       | O(n)       | O(1)*  | O(1)*  | Mid-list mutations*
Stack<T>            | O(1)       | O(1)   | O(1)   | LIFO, parse
Queue<T>            | O(1)       | O(1)   | O(1)   | FIFO, scheduling
ArrayPool<T>        | O(1)       | O(1)   | O(1)   | Temp large buffers

* If you have the node reference; otherwise O(n) to find it
");

        Console.WriteLine("Rule of Thumb:");
        Console.WriteLine("  • Use List<T> by default for general-purpose collections");
        Console.WriteLine("  • Use Dictionary<K,V> when you need fast key lookups");
        Console.WriteLine("  • Use HashSet<T> for membership tests (never List<T>.Contains in a loop!)");
        Console.WriteLine("  • Use Stack<T>/Queue<T> for LIFO/FIFO semantics");
        Console.WriteLine("  • Use LinkedList<T> only if you have node references for O(1) insertions");
    }
}
