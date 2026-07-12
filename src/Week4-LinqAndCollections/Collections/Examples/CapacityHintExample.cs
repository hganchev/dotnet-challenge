namespace Collections.Examples;

/// <summary>
/// Demonstrates capacity hints when initializing collections to avoid
/// repeated internal resizing and improve performance.
/// </summary>
public static class CapacityHintExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Capacity Hints for Performance ---");

        const int itemCount = 2_000_000;
        const int itemsForDict = itemCount / 10;

        // Warm up the JIT for both code paths once before measuring, and take
        // the best of several trials — a single timed run is noisy (GC pauses,
        // OS scheduling, JIT tiering) and can easily show a "hint" run as
        // slower purely by chance.
        FillList(itemCount, withCapacityHint: false);
        FillList(itemCount, withCapacityHint: true);

        Console.WriteLine($"Adding {itemCount} items without capacity hint:");
        var (noHintMs, noHintCapacity) = BestOf(3, () => FillList(itemCount, withCapacityHint: false));
        Console.WriteLine($"  Best time: {noHintMs} ms, Final capacity: {noHintCapacity}");

        Console.WriteLine($"\nAdding {itemCount} items WITH capacity hint:");
        var (hintMs, hintCapacity) = BestOf(3, () => FillList(itemCount, withCapacityHint: true));
        Console.WriteLine($"  Best time: {hintMs} ms, Final capacity: {hintCapacity}");

        // Dictionary with capacity hint
        Console.WriteLine($"\nAdding {itemsForDict} items to Dictionary:");
        FillDictionary(itemsForDict, withCapacityHint: false);
        FillDictionary(itemsForDict, withCapacityHint: true);

        var (dictNoHintUs, _) = BestOfMicroseconds(3, () => FillDictionary(itemsForDict, withCapacityHint: false));
        Console.WriteLine($"  Without hint - Best time: {dictNoHintUs:F0} us");

        var (dictHintUs, _) = BestOfMicroseconds(3, () => FillDictionary(itemsForDict, withCapacityHint: true));
        Console.WriteLine($"  With hint - Best time: {dictHintUs:F0} us");

        Console.WriteLine("\nConclusion: Use capacity hints when you know the size!");
    }

    private static (long ElapsedMs, int Capacity) FillList(int itemCount, bool withCapacityHint)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var list = withCapacityHint ? new List<int>(itemCount) : new List<int>();
        for (int i = 0; i < itemCount; i++)
        {
            list.Add(i);
        }
        sw.Stop();
        return (sw.ElapsedMilliseconds, list.Capacity);
    }

    private static double FillDictionary(int itemCount, bool withCapacityHint)
    {
        // Use a plain int value (not a formatted string) so the measurement
        // isolates dictionary resize/rehash cost rather than string allocation cost.
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var dict = withCapacityHint ? new Dictionary<int, int>(itemCount) : new Dictionary<int, int>();
        for (int i = 0; i < itemCount; i++)
        {
            dict[i] = i;
        }
        sw.Stop();
        return sw.Elapsed.TotalMicroseconds;
    }

    private static (long BestMs, int Capacity) BestOf(int trials, Func<(long ElapsedMs, int Capacity)> action)
    {
        long bestMs = long.MaxValue;
        int capacity = 0;
        for (int i = 0; i < trials; i++)
        {
            var (elapsedMs, cap) = action();
            if (elapsedMs < bestMs)
            {
                bestMs = elapsedMs;
                capacity = cap;
            }
        }
        return (bestMs, capacity);
    }

    private static (double BestUs, int Unused) BestOfMicroseconds(int trials, Func<double> action)
    {
        double bestUs = double.MaxValue;
        for (int i = 0; i < trials; i++)
        {
            var us = action();
            if (us < bestUs)
                bestUs = us;
        }
        return (bestUs, 0);
    }
}
