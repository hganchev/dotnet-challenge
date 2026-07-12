using System.Collections.Immutable;

namespace Collections.Examples;

/// <summary>
/// Demonstrates ImmutableArray<T> and ImmutableDictionary<K,V> for thread-safe
/// sharing of collections without locks, using structural sharing.
/// </summary>
public static class ImmutableCollectionsExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Immutable Collections ---");

        // ImmutableArray<T>
        Console.WriteLine("ImmutableArray<T>:");
        var immArr = new[] { 1, 2, 3, 4, 5 }.ToImmutableArray();
        Console.WriteLine($"Initial: {string.Join(", ", immArr)}");

        var immArr2 = immArr.Add(6);
        Console.WriteLine($"After Add(6): {string.Join(", ", immArr2)}");
        Console.WriteLine($"Original unchanged: {string.Join(", ", immArr)}");

        var immArr3 = immArr2.RemoveAt(0);
        Console.WriteLine($"After RemoveAt(0): {string.Join(", ", immArr3)}");

        // ImmutableDictionary<K,V>
        Console.WriteLine("\nImmutableDictionary<K, V>:");
        var immDict = new Dictionary<string, int>
        {
            ["Alice"] = 85,
            ["Bob"] = 92,
        }.ToImmutableDictionary();

        Console.WriteLine($"Initial: {string.Join(", ", immDict.Select(kv => $"{kv.Key}={kv.Value}"))}");

        var immDict2 = immDict.Add("Charlie", 78);
        Console.WriteLine($"After Add: {string.Join(", ", immDict2.Select(kv => $"{kv.Key}={kv.Value}"))}");
        Console.WriteLine($"Original unchanged: {string.Join(", ", immDict.Select(kv => $"{kv.Key}={kv.Value}"))}");

        var immDict3 = immDict2.Remove("Bob");
        Console.WriteLine($"After Remove: {string.Join(", ", immDict3.Select(kv => $"{kv.Key}={kv.Value}"))}");

        // Thread safety
        Console.WriteLine("\nThread Safety:");
        Console.WriteLine("Immutable collections are safe to share across threads without locks");
        Console.WriteLine("Modifications create new collections via structural sharing");
        Console.WriteLine("(Old collection remains unchanged and can be used by other threads)");
    }
}
