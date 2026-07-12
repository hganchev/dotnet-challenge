namespace Collections.Examples;

/// <summary>
/// Demonstrates Dictionary<K,V> — fast key lookups, hash table based,
/// iteration order is undefined (unlike some other languages' linked hash maps).
/// </summary>
public static class DictionaryExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Dictionary<K, V> ---");

        var dict = new Dictionary<string, int>
        {
            ["Alice"] = 85,
            ["Bob"] = 92,
            ["Charlie"] = 78,
        };

        Console.WriteLine($"Initial: {string.Join(", ", dict.Select(kv => $"{kv.Key}={kv.Value}"))}");

        // Lookup
        Console.WriteLine($"\ndict[\"Bob\"] = {dict["Bob"]}");

        // TryGetValue (safer)
        if (dict.TryGetValue("Alice", out var aliceScore))
            Console.WriteLine($"Alice's score: {aliceScore}");

        // Add
        dict["Diana"] = 95;
        Console.WriteLine($"After adding Diana: {dict.Count} items");

        // ContainsKey
        Console.WriteLine($"Contains \"Charlie\": {dict.ContainsKey("Charlie")}");

        // Remove
        dict.Remove("Bob");
        Console.WriteLine($"After removing Bob: {dict.Count} items");

        // Iteration (order not guaranteed)
        Console.WriteLine($"\nIterating (order undefined):");
        foreach (var kvp in dict)
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
        }

        // GroupBy into Dictionary
        Console.WriteLine("\nGrouping into Dictionary:");
        var scores = new[] { 85, 92, 78, 95, 88 };
        var groups = new Dictionary<string, List<int>>
        {
            ["Pass"] = new(),
            ["Fail"] = new(),
        };

        foreach (var score in scores)
        {
            var category = score >= 80 ? "Pass" : "Fail";
            groups[category].Add(score);
        }

        foreach (var group in groups)
        {
            Console.WriteLine($"  {group.Key}: {string.Join(", ", group.Value)}");
        }
    }
}
