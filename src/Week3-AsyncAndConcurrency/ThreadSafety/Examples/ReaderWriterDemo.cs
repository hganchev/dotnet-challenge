namespace ThreadSafety.Examples;

/// <summary>
/// Demonstrates ReaderWriterLockSlim for scenarios where reads vastly
/// outnumber writes: multiple concurrent readers, exclusive writers.
/// </summary>
public static class ReaderWriterDemo
{
    public static void Run()
    {
        Console.WriteLine("=== ReaderWriterLockSlim Demo ===\n");

        BasicReaderWriterDemo();
        UpgradeableReadDemo();
        ThroughputComparisonDemo();
    }

    // -------------------------------------------------------------------------
    // 1. Multiple concurrent readers, exclusive writer
    // -------------------------------------------------------------------------
    private static void BasicReaderWriterDemo()
    {
        Console.WriteLine("--- Multiple readers / exclusive writer ---");

        var rwLock = new ReaderWriterLockSlim();
        var cache  = new Dictionary<string, string>();

        void Read(string key)
        {
            rwLock.EnterReadLock();
            try
            {
                var found = cache.TryGetValue(key, out var value);
                Console.WriteLine($"  [T{Thread.CurrentThread.ManagedThreadId}] Read  '{key}' → {(found ? value : "miss")}");
            }
            finally { rwLock.ExitReadLock(); }
        }

        void Write(string key, string value)
        {
            rwLock.EnterWriteLock();
            try
            {
                cache[key] = value;
                Console.WriteLine($"  [T{Thread.CurrentThread.ManagedThreadId}] Write '{key}' = {value}");
            }
            finally { rwLock.ExitWriteLock(); }
        }

        // Pre-populate
        Write("a", "alpha");
        Write("b", "beta");

        // Concurrent readers
        var readers = Enumerable.Range(1, 4)
            .Select(_ => new Thread(() => { Read("a"); Thread.Sleep(5); Read("b"); }))
            .ToArray();

        var writer = new Thread(() => { Thread.Sleep(10); Write("c", "gamma"); });

        foreach (var r in readers) r.Start();
        writer.Start();
        foreach (var r in readers) r.Join();
        writer.Join();
        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 2. Upgradeable read lock — check-then-act without releasing the lock
    //    Pattern: enter upgradeable read → check condition → if needed, upgrade to write.
    // -------------------------------------------------------------------------
    private static void UpgradeableReadDemo()
    {
        Console.WriteLine("--- Upgradeable read lock ---");

        var rwLock = new ReaderWriterLockSlim();
        var cache  = new Dictionary<string, int>();

        void GetOrAdd(string key, int value)
        {
            // Upgradeable read: other readers can still read concurrently,
            // but only one thread can hold an upgradeable read at a time.
            rwLock.EnterUpgradeableReadLock();
            try
            {
                if (cache.ContainsKey(key))
                {
                    Console.WriteLine($"  Exists: {key}={cache[key]}");
                    return;
                }

                // Upgrade to write lock only if we need to insert
                rwLock.EnterWriteLock();
                try
                {
                    cache[key] = value; // double-check inside write lock in production
                    Console.WriteLine($"  Inserted: {key}={value}");
                }
                finally { rwLock.ExitWriteLock(); }
            }
            finally { rwLock.ExitUpgradeableReadLock(); }
        }

        GetOrAdd("x", 10);
        GetOrAdd("y", 20);
        GetOrAdd("x", 99); // already exists — no write lock acquired

        Console.WriteLine();
    }

    // -------------------------------------------------------------------------
    // 3. Throughput comparison: lock vs ReaderWriterLockSlim for read-heavy load
    // -------------------------------------------------------------------------
    private static void ThroughputComparisonDemo()
    {
        Console.WriteLine("--- Throughput: lock vs ReaderWriterLockSlim (read-heavy) ---");

        const int ReadThreads  = 8;
        const int WriteThreads = 1;
        const int OpsPerThread = 50_000;

        // --- plain lock ---
        var plainLock = new object();
        var plainDict = new Dictionary<int, int> { [1] = 0 };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        RunWorkload(
            readAction:  () => { lock (plainLock) { _ = plainDict[1]; } },
            writeAction: () => { lock (plainLock) { plainDict[1]++; } },
            ReadThreads, WriteThreads, OpsPerThread);
        var plainMs = sw.ElapsedMilliseconds;

        // --- ReaderWriterLockSlim ---
        var rwLock = new ReaderWriterLockSlim();
        var rwDict = new Dictionary<int, int> { [1] = 0 };

        sw.Restart();
        RunWorkload(
            readAction: () =>
            {
                rwLock.EnterReadLock();
                try { _ = rwDict[1]; }
                finally { rwLock.ExitReadLock(); }
            },
            writeAction: () =>
            {
                rwLock.EnterWriteLock();
                try { rwDict[1]++; }
                finally { rwLock.ExitWriteLock(); }
            },
            ReadThreads, WriteThreads, OpsPerThread);
        var rwMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"  lock               : {plainMs} ms ({ReadThreads} readers, {WriteThreads} writer)");
        Console.WriteLine($"  ReaderWriterLockSlim: {rwMs} ms ({ReadThreads} readers, {WriteThreads} writer)");
        Console.WriteLine("  → RWLS wins when reads >> writes; overhead outweighs gain for write-heavy loads.\n");
    }

    private static void RunWorkload(
        Action readAction, Action writeAction,
        int readThreads, int writeThreads, int opsPerThread)
    {
        var threads = new List<Thread>();

        for (var i = 0; i < readThreads; i++)
        {
            var t = new Thread(() => { for (var n = 0; n < opsPerThread; n++) readAction(); });
            threads.Add(t);
        }
        for (var i = 0; i < writeThreads; i++)
        {
            var t = new Thread(() => { for (var n = 0; n < opsPerThread; n++) writeAction(); });
            threads.Add(t);
        }

        foreach (var t in threads) t.Start();
        foreach (var t in threads) t.Join();
    }
}
