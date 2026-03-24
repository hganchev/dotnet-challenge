using ThreadSafety.Concepts;

namespace ThreadSafety.Examples;

/// <summary>
/// Demonstrates Interlocked atomic operations: Increment/Decrement,
/// Add, Exchange, CompareExchange, and a lock-free spin loop.
/// Compares performance with the lock-based alternative.
/// </summary>
public static class InterlockedDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Interlocked Demo ===\n");

        AtomicCounterDemo();
        CompareExchangeDemo();
        LockFreeStackDemo();
        PerformanceComparisonDemo();
    }

    // -------------------------------------------------------------------------
    // 1. Interlocked.Increment/Decrement — safe counter without lock
    // -------------------------------------------------------------------------
    private static void AtomicCounterDemo()
    {
        Console.WriteLine("--- Interlocked.Increment / Decrement ---");

        const int Iterations = 100_000;
        var counter = new SharedCounter();

        var t1 = new Thread(() => { for (var i = 0; i < Iterations; i++) counter.AtomicIncrement(); });
        var t2 = new Thread(() => { for (var i = 0; i < Iterations; i++) counter.AtomicIncrement(); });

        t1.Start(); t2.Start();
        t1.Join();  t2.Join();

        Console.WriteLine($"  Expected : {Iterations * 2:N0}");
        Console.WriteLine($"  Actual   : {counter.AtomicValue:N0}  ← always correct, no lock\n");
    }

    // -------------------------------------------------------------------------
    // 2. Interlocked.CompareExchange — the foundation of all lock-free algorithms
    //    Atomically: if current == comparand, set to newValue; return original.
    // -------------------------------------------------------------------------
    private static void CompareExchangeDemo()
    {
        Console.WriteLine("--- Interlocked.CompareExchange ---");

        int state = 0; // 0 = idle, 1 = busy

        // Simulate acquiring a "spinlock" flag
        var previous = Interlocked.CompareExchange(ref state, value: 1, comparand: 0);
        Console.WriteLine($"  First CAS  → previous={previous}, state={state}  (acquired)");

        // Second attempt fails because state is already 1
        previous = Interlocked.CompareExchange(ref state, value: 1, comparand: 0);
        Console.WriteLine($"  Second CAS → previous={previous}, state={state}  (already held)\n");

        // Long (64-bit) version on 32-bit platforms requires Interlocked too
        long longVal = 0;
        Interlocked.Add(ref longVal, 100);
        Console.WriteLine($"  Interlocked.Add on long: {longVal}\n");
    }

    // -------------------------------------------------------------------------
    // 3. Lock-free stack using CompareExchange (Treiber stack)
    //    Classic example of building a thread-safe data structure without lock.
    // -------------------------------------------------------------------------
    private static void LockFreeStackDemo()
    {
        Console.WriteLine("--- Lock-free stack (Treiber stack) ---");

        var stack = new LockFreeStack<int>();

        // Push from multiple threads
        var pushers = Enumerable.Range(1, 4)
            .Select(n => new Thread(() =>
            {
                for (var i = 0; i < 3; i++)
                    stack.Push(n * 10 + i);
            })).ToArray();

        foreach (var t in pushers) t.Start();
        foreach (var t in pushers) t.Join();

        Console.Write("  Popped: ");
        while (stack.TryPop(out var val))
            Console.Write($"{val} ");
        Console.WriteLine("\n");
    }

    // -------------------------------------------------------------------------
    // 4. Performance comparison: Interlocked vs lock for a simple counter
    // -------------------------------------------------------------------------
    private static void PerformanceComparisonDemo()
    {
        Console.WriteLine("--- Performance: Interlocked vs lock (10M increments) ---");

        const int N = 10_000_000;
        int lockedVal = 0, atomicVal = 0;
        var lockObj = new object();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        Parallel.For(0, N, _ => { lock (lockObj) { lockedVal++; } });
        var lockMs = sw.ElapsedMilliseconds;

        sw.Restart();
        Parallel.For(0, N, _ => Interlocked.Increment(ref atomicVal));
        var atomicMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"  lock        : {lockMs} ms  ({lockedVal:N0})");
        Console.WriteLine($"  Interlocked : {atomicMs} ms  ({atomicVal:N0})");
        Console.WriteLine("  → Interlocked is significantly faster for simple increment.\n");
    }
}

// ---------------------------------------------------------------------------
// Lock-free stack using Interlocked.CompareExchange (Treiber stack algorithm)
// ---------------------------------------------------------------------------

file sealed class LockFreeStack<T>
{
    private sealed class Node(T value, Node? next)
    {
        public readonly T Value = value;
        public Node? Next = next;
    }

    private Node? _head;

    public void Push(T value)
    {
        var node = new Node(value, null);
        Node? current;
        do
        {
            current = _head;
            node.Next = current;
        }
        while (Interlocked.CompareExchange(ref _head, node, current) != current);
    }

    public bool TryPop(out T value)
    {
        Node? current;
        do
        {
            current = _head;
            if (current is null) { value = default!; return false; }
        }
        while (Interlocked.CompareExchange(ref _head, current.Next, current) != current);

        value = current.Value;
        return true;
    }
}
