using System.Collections.Concurrent;

namespace ThreadSafety.Examples;

/// <summary>
/// Demonstrates the thread-safe collections in System.Collections.Concurrent:
/// ConcurrentDictionary, ConcurrentQueue, ConcurrentBag, and BlockingCollection.
/// </summary>
public static class ConcurrentCollectionsDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Concurrent Collections Demo ===\n");

        ConcurrentDictionaryDemo();
        ConcurrentQueueDemo();
        ConcurrentBagDemo();
        BlockingCollectionDemo();
    }

    // -------------------------------------------------------------------------
    // 1. ConcurrentDictionary — thread-safe map with atomic factory methods
    // -------------------------------------------------------------------------
    private static void ConcurrentDictionaryDemo()
    {
        Console.WriteLine("--- ConcurrentDictionary<K,V> ---");

        var counts = new ConcurrentDictionary<string, int>();
        var words = new[] { "apple", "banana", "apple", "cherry", "banana", "apple" };

        // Parallel word-count using AddOrUpdate — atomic, no external lock needed
        Parallel.ForEach(words, word =>
            counts.AddOrUpdate(word, addValue: 1, updateValueFactory: (_, old) => old + 1));

        foreach (var (word, count) in counts.OrderBy(kv => kv.Key))
            Console.WriteLine($"  {word}: {count}");

        // GetOrAdd — returns existing or inserts via factory (factory runs at most once per key)
        var langVersion = counts.GetOrAdd("dotnet", _ => 9);
        Console.WriteLine($"  GetOrAdd 'dotnet': {langVersion}");

        // TryGetValue / TryRemove
        if (counts.TryRemove("banana", out var removed))
            Console.WriteLine($"  Removed 'banana' (was {removed})");
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 2. ConcurrentQueue<T> — thread-safe FIFO, lock-free
    // -------------------------------------------------------------------------
    private static void ConcurrentQueueDemo()
    {
        Console.WriteLine("--- ConcurrentQueue<T> ---");

        var queue = new ConcurrentQueue<string>();

        // Multiple producers enqueue concurrently
        Parallel.For(1, 6, i => queue.Enqueue($"item-{i}"));

        Console.Write("  Dequeued: ");
        while (queue.TryDequeue(out var item))
            Console.Write($"{item} ");
        Console.WriteLine($"\n  Queue empty: {queue.IsEmpty}\n");
    }

    // -------------------------------------------------------------------------
    // 3. ConcurrentBag<T> — unordered, optimised for producer=consumer thread
    //    Internally uses per-thread lists; best when the same thread both adds and removes.
    // -------------------------------------------------------------------------
    private static void ConcurrentBagDemo()
    {
        Console.WriteLine("--- ConcurrentBag<T> ---");

        var bag = new ConcurrentBag<int>();

        Parallel.For(0, 10, i => bag.Add(i * i));

        Console.Write("  Items (unordered): ");
        foreach (var item in bag.OrderBy(x => x))
            Console.Write($"{item} ");
        Console.WriteLine($"\n  Count: {bag.Count}\n");
    }

    // -------------------------------------------------------------------------
    // 4. BlockingCollection<T> — producer/consumer with bounded capacity
    //    Blocks producers when full; blocks consumers when empty.
    //    Note: prefer Channel<T> in async code — BlockingCollection blocks threads.
    // -------------------------------------------------------------------------
    private static void BlockingCollectionDemo()
    {
        Console.WriteLine("--- BlockingCollection<T> (bounded, thread-blocking) ---");

        using var collection = new BlockingCollection<int>(boundedCapacity: 3);

        var producer = new Thread(() =>
        {
            for (var i = 1; i <= 6; i++)
            {
                collection.Add(i); // blocks if full
                Console.WriteLine($"  Produced: {i}  (count={collection.Count})");
                Thread.Sleep(10);
            }
            collection.CompleteAdding();
        });

        var consumer = new Thread(() =>
        {
            foreach (var item in collection.GetConsumingEnumerable())
            {
                Thread.Sleep(25); // slower than producer — demonstrates blocking
                Console.WriteLine($"  Consumed: {item}");
            }
        });

        producer.Start();
        consumer.Start();
        producer.Join();
        consumer.Join();

        Console.WriteLine("  → For async pipelines prefer Channel<T> over BlockingCollection<T>\n");
    }
}
