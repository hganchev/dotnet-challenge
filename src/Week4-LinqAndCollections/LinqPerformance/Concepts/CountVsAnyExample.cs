namespace LinqPerformance.Concepts;

/// <summary>
/// Demonstrates that Count() > 0 is slower than Any() because Count scans
/// the entire sequence, while Any() short-circuits after the first match.
/// </summary>
public static class CountVsAnyExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Count() vs Any() ---");

        var itemCount = 10_000_000;

        // IMPORTANT: Enumerable.Range() implements IPartition<T>/IIListProvider<T>,
        // which gives Count() an O(1) fast path (the count is known analytically) —
        // it never actually enumerates! To see the real Count() vs Any() difference,
        // wrap the sequence in a Where() so the fast path can't apply (a predicate
        // must be evaluated per item, so the count can't be derived cheaply).
        Console.WriteLine($"Sequence with {itemCount} items (wrapped in Where to defeat the Count() fast path):");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var hasAny1 = Enumerable.Range(1, itemCount).Where(n => n >= 1).Count() > 0;
        sw.Stop();
        Console.WriteLine($"Count() > 0:        {sw.Elapsed.TotalMicroseconds:F0} us, Result: {hasAny1}");

        // Any() — short-circuits after first item
        sw.Restart();
        var hasAny2 = Enumerable.Range(1, itemCount).Where(n => n >= 1).Any();
        sw.Stop();
        Console.WriteLine($"Any():              {sw.Elapsed.TotalMicroseconds:F0} us, Result: {hasAny2}");

        // With filtering that matches EARLY (first item) — Any() short-circuits, Count() must scan all
        Console.WriteLine($"\nPredicate matches immediately (n > 0) — Any() should win big:");
        sw.Restart();
        var hasMatch1 = Enumerable.Range(1, itemCount).Count(n => n > 0) > 0; // matches every item, still scans all
        sw.Stop();
        Console.WriteLine($"Count(predicate) > 0: {sw.Elapsed.TotalMicroseconds:F0} us");

        sw.Restart();
        var hasMatch2 = Enumerable.Range(1, itemCount).Any(n => n > 0); // short-circuits after item 1
        sw.Stop();
        Console.WriteLine($"Any(predicate):       {sw.Elapsed.TotalMicroseconds:F0} us");

        // Worst case: no match at all — both Count() and Any() must scan the entire sequence
        Console.WriteLine($"\nPredicate never matches (worst case) — both must scan everything:");
        sw.Restart();
        var hasMatch3 = Enumerable.Range(1, itemCount).Count(n => n > itemCount) > 0;
        sw.Stop();
        Console.WriteLine($"Count(predicate) > 0: {sw.Elapsed.TotalMicroseconds:F0} us");

        sw.Restart();
        var hasMatch4 = Enumerable.Range(1, itemCount).Any(n => n > itemCount);
        sw.Stop();
        Console.WriteLine($"Any(predicate):       {sw.Elapsed.TotalMicroseconds:F0} us");

        Console.WriteLine("\nAlways use Any() for boolean checks!");
    }
}
