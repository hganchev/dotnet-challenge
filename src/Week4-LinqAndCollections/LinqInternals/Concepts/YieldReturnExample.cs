namespace LinqInternals.Concepts;

/// <summary>
/// Demonstrates yield return and how it implements IEnumerable efficiently,
/// allocating only the enumerator on demand.
/// </summary>
public static class YieldReturnExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Yield Return (Custom IEnumerable) ---");

        Console.WriteLine("Fibonacci sequence (first 10 numbers):");
        var fib = Fibonacci().Take(10);

        foreach (var num in fib)
        {
            Console.WriteLine(num);
        }

        Console.WriteLine("\nFibonacci again (lazy evaluation, no stored state):");
        var fib2 = Fibonacci().Take(5);
        foreach (var num in fib2)
        {
            Console.WriteLine(num);
        }

        // Custom iterator with filtering
        Console.WriteLine("\nFiltered Fibonacci (> 1000):");
        var largeFilib = FilteredFibonacci(x => x > 1000).Take(5);
        foreach (var num in largeFilib)
        {
            Console.WriteLine(num);
        }
    }

    /// <summary>
    /// Returns an infinite Fibonacci sequence using yield return.
    /// The sequence is generated on-demand, one number at a time.
    /// </summary>
    private static IEnumerable<long> Fibonacci()
    {
        long a = 0, b = 1;
        while (true)
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }

    /// <summary>
    /// Returns a filtered Fibonacci sequence with predicate applied.
    /// Demonstrates yield return with additional processing.
    /// </summary>
    private static IEnumerable<long> FilteredFibonacci(Func<long, bool> predicate)
    {
        long a = 0, b = 1;
        while (true)
        {
            if (predicate(a))
            {
                yield return a;
            }
            (a, b) = (b, a + b);
        }
    }
}
