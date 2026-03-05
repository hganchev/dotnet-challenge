namespace Generics.Examples;

public static class GenericCollections
{
    public static void Run()
    {
        Console.WriteLine("\n=== Generic Collections ===");

        // List<T> — ordered, index-accessible, allows duplicates
        var list = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6 };
        list.Sort();
        Console.WriteLine($"List<int> sorted: [{string.Join(", ", list)}]");
        list.RemoveAll(x => x % 2 == 0);
        Console.WriteLine($"After RemoveAll(even): [{string.Join(", ", list)}]");

        // Dictionary<TKey, TValue> — key-value store, O(1) lookup
        var wordCount = new Dictionary<string, int>();
        foreach (var word in "the quick brown fox jumps over the lazy dog the".Split(' '))
            wordCount[word] = wordCount.GetValueOrDefault(word) + 1;

        Console.WriteLine("Dictionary<string, int> word counts:");
        foreach (var (word, count) in wordCount.OrderByDescending(kv => kv.Value).Take(3))
            Console.WriteLine($"  '{word}': {count}");

        // HashSet<T> — unordered, unique elements, O(1) contains
        var setA = new HashSet<int> { 1, 2, 3, 4, 5 };
        var setB = new HashSet<int> { 3, 4, 5, 6, 7 };

        var intersection = new HashSet<int>(setA);
        intersection.IntersectWith(setB);
        Console.WriteLine($"HashSet intersection: [{string.Join(", ", intersection)}]");

        var union = new HashSet<int>(setA);
        union.UnionWith(setB);
        Console.WriteLine($"HashSet union: [{string.Join(", ", union.Order())}]");

        // Stack<T> and Queue<T>
        var stack = new Stack<string>(["first", "second", "third"]);
        Console.WriteLine($"Stack<string> pop: {stack.Pop()}");

        var queue = new Queue<string>(["first", "second", "third"]);
        Console.WriteLine($"Queue<string> dequeue: {queue.Dequeue()}");
    }
}
