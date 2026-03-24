using ThreadSafety.Concepts;

namespace ThreadSafety.Examples;

/// <summary>
/// Demonstrates race conditions on unsynchronised state, the lock statement
/// as the fix, and Monitor.Wait/Pulse for thread signalling.
/// </summary>
public static class LockDemo
{
    public static void Run()
    {
        Console.WriteLine("=== Lock / Monitor Demo ===\n");

        RaceConditionDemo();
        LockFixDemo();
        MonitorWaitPulseDemo();
        DeadlockAvoidanceDemo();
    }

    // -------------------------------------------------------------------------
    // 1. Race condition — unsynchronised increment loses updates
    // -------------------------------------------------------------------------
    private static void RaceConditionDemo()
    {
        Console.WriteLine("--- Race condition (no synchronisation) ---");

        const int Iterations = 100_000;
        var counter = new SharedCounter();

        var t1 = new Thread(() => { for (var i = 0; i < Iterations; i++) counter.UnsafeIncrement(); });
        var t2 = new Thread(() => { for (var i = 0; i < Iterations; i++) counter.UnsafeIncrement(); });

        t1.Start(); t2.Start();
        t1.Join();  t2.Join();

        Console.WriteLine($"  Expected : {Iterations * 2:N0}");
        Console.WriteLine($"  Actual   : {counter.UnsafeValue:N0}  ← lost updates due to race condition");
        Console.WriteLine($"  Deficit  : {Iterations * 2 - counter.UnsafeValue:N0}\n");
    }

    // -------------------------------------------------------------------------
    // 2. Lock fixes the race — every update is visible
    // -------------------------------------------------------------------------
    private static void LockFixDemo()
    {
        Console.WriteLine("--- lock statement fixes the race ---");

        const int Iterations = 100_000;
        var counter = new SharedCounter();

        var t1 = new Thread(() => { for (var i = 0; i < Iterations; i++) counter.LockedIncrement(); });
        var t2 = new Thread(() => { for (var i = 0; i < Iterations; i++) counter.LockedIncrement(); });

        t1.Start(); t2.Start();
        t1.Join();  t2.Join();

        Console.WriteLine($"  Expected : {Iterations * 2:N0}");
        Console.WriteLine($"  Actual   : {counter.LockedValue:N0}  ← always correct\n");
    }

    // -------------------------------------------------------------------------
    // 3. Monitor.Wait / Pulse — producer signals consumer when data is ready
    //    Equivalent to a manual condition variable.
    // -------------------------------------------------------------------------
    private static void MonitorWaitPulseDemo()
    {
        Console.WriteLine("--- Monitor.Wait / Pulse (condition variable) ---");

        var _lock = new object();
        var _queue = new Queue<int>();
        var _done = false;

        var consumer = new Thread(() =>
        {
            lock (_lock)
            {
                while (true)
                {
                    // Wait until there is something to consume (or producer is done)
                    while (_queue.Count == 0 && !_done)
                        Monitor.Wait(_lock);

                    while (_queue.Count > 0)
                        Console.Write($" consumed:{_queue.Dequeue()}");

                    if (_done) break;
                }
            }
            Console.WriteLine("\n  Consumer finished");
        });

        consumer.Start();

        // Producer: enqueue items and signal
        for (var i = 1; i <= 5; i++)
        {
            Thread.Sleep(15);
            lock (_lock)
            {
                _queue.Enqueue(i);
                Monitor.Pulse(_lock); // wake consumer
            }
        }

        lock (_lock)
        {
            _done = true;
            Monitor.Pulse(_lock);
        }

        consumer.Join();
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 4. Deadlock avoidance — always acquire locks in the same order
    // -------------------------------------------------------------------------
    private static void DeadlockAvoidanceDemo()
    {
        Console.WriteLine("--- Deadlock avoidance (consistent lock order) ---");

        var lockA = new object();
        var lockB = new object();

        // SAFE: both threads acquire A then B — no deadlock possible
        var t1 = new Thread(() =>
        {
            lock (lockA) { Thread.Sleep(5); lock (lockB) { Console.WriteLine("  T1: acquired A then B"); } }
        });

        var t2 = new Thread(() =>
        {
            lock (lockA) { Thread.Sleep(5); lock (lockB) { Console.WriteLine("  T2: acquired A then B"); } }
        });

        t1.Start(); t2.Start();
        t1.Join();  t2.Join();

        Console.WriteLine("  Rule: always acquire multiple locks in the same global order.\n");
    }
}
