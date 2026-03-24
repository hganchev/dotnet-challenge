using System.Collections.Immutable;
using ThreadSafety.Concepts;

namespace ThreadSafety.Examples;

/// <summary>
/// Demonstrates how immutable data structures enable safe sharing across
/// threads with zero synchronisation — no locks, no races possible.
/// </summary>
public static class ImmutabilityDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Immutability Demo ===\n");

        ImmutableRecordDemo();
        ImmutableCollectionsDemo();
        SnapshotPatternDemo();
        ThreadLocalDemo.Run();
    }

    // -------------------------------------------------------------------------
    // 1. Immutable record — once created, safe to share across any threads
    // -------------------------------------------------------------------------
    private static void ImmutableRecordDemo()
    {
        Console.WriteLine("--- Immutable record ---");

        var config = new AppConfig("production", MaxConnections: 50, TimeoutSeconds: 30);
        Console.WriteLine($"  Original: {config}");

        // 'with' creates a new instance — original is unchanged
        var testConfig = config with { Environment = "testing", MaxConnections = 5 };
        Console.WriteLine($"  Modified: {testConfig}");
        Console.WriteLine($"  Original unchanged: {config}");

        // Share config across multiple threads safely — no lock needed
        var results = new string[4];
        var threads = Enumerable.Range(0, 4)
            .Select(i => new Thread(() =>
            {
                // Each thread reads config safely — immutable means no data races
                results[i] = $"T{i}: env={config.Environment}, max={config.MaxConnections}";
            })).ToArray();

        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();

        foreach (var r in results) Console.WriteLine($"  {r}");
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 2. ImmutableList / ImmutableDictionary — safe shared read-only state
    //    Mutations return a new collection; original is untouched.
    // -------------------------------------------------------------------------
    private static void ImmutableCollectionsDemo()
    {
        Console.WriteLine("--- ImmutableList / ImmutableDictionary ---");

        var list = ImmutableList.Create(1, 2, 3, 4, 5);
        var added = list.Add(6);
        var removed = list.Remove(3);

        Console.WriteLine($"  Original : [{string.Join(", ", list)}]");
        Console.WriteLine($"  After Add: [{string.Join(", ", added)}]");
        Console.WriteLine($"  After Remove 3: [{string.Join(", ", removed)}]");

        var dict = ImmutableDictionary<string, int>.Empty
            .Add("a", 1)
            .Add("b", 2)
            .SetItem("a", 99); // returns new dict; original untouched

        Console.WriteLine($"  Dict: a={dict["a"]}, b={dict["b"]}");
        Console.WriteLine("  (All mutations return new instances; originals are unchanged)\n");
    }

    // -------------------------------------------------------------------------
    // 3. Snapshot pattern — publish an immutable snapshot via volatile field
    //    Writers create a new snapshot; readers always see a consistent whole.
    //    Avoids locking for readers entirely.
    // -------------------------------------------------------------------------
    private static void SnapshotPatternDemo()
    {
        Console.WriteLine("--- Snapshot pattern (publish via volatile reference) ---");

        var store = new SnapshotStore();

        // Readers on background threads
        var readCount = 0;
        var readers = Enumerable.Range(0, 4).Select(_ => new Thread(() =>
        {
            for (var i = 0; i < 100; i++)
            {
                var snap = store.Snapshot; // atomic reference read
                _ = snap.Items.Count;      // safe — snapshot is immutable
                Interlocked.Increment(ref readCount);
                Thread.SpinWait(100);
            }
        })).ToArray();

        // Writer on main thread
        foreach (var t in readers) t.Start();
        for (var i = 0; i < 5; i++)
        {
            Thread.Sleep(10);
            store.AddItem($"item-{i}");
            Console.WriteLine($"  Published snapshot v{i + 1}: {store.Snapshot.Items.Count} items");
        }

        foreach (var t in readers) t.Join();
        Console.WriteLine($"  Total reads: {readCount} (zero reader locks needed)\n");
    }
}

// ---------------------------------------------------------------------------
// Supporting types
// ---------------------------------------------------------------------------

file sealed record AppConfig(string Environment, int MaxConnections, int TimeoutSeconds);

file sealed class DataSnapshot(ImmutableList<string> items)
{
    public ImmutableList<string> Items { get; } = items;
}

file sealed class SnapshotStore
{
    private volatile DataSnapshot _current = new(ImmutableList<string>.Empty);

    public DataSnapshot Snapshot => _current;

    public void AddItem(string item)
    {
        // Spin until our CAS succeeds — handles concurrent writers
        while (true)
        {
            var current = _current;
            var next    = new DataSnapshot(current.Items.Add(item));
            if (Interlocked.CompareExchange(ref _current, next, current) == current)
                break;
        }
    }
}
