namespace LinqPerformance.Examples;

/// <summary>
/// Demonstrates PLINQ (Parallel LINQ) with .AsParallel() for
/// CPU-bound queries across multiple cores.
/// Shows when PLINQ helps and when overhead hurts.
/// </summary>
public static class ParallelLinqExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Parallel LINQ (.AsParallel()) ---");

        // CPU-bound operation: Sieve of Eratosthenes
        var range = 100_000;

        // Sequential
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var sequential = Enumerable.Range(2, range)
            .Where(IsPrime)
            .Count();
        sw.Stop();
        Console.WriteLine($"Sequential:  {sw.ElapsedMilliseconds} ms, Primes: {sequential}");

        // Parallel
        sw.Restart();
        var parallel = Enumerable.Range(2, range)
            .AsParallel()
            .Where(IsPrime)
            .Count();
        sw.Stop();
        Console.WriteLine($"Parallel:    {sw.ElapsedMilliseconds} ms, Primes: {parallel}");

        // Small dataset (overhead hurts)
        Console.WriteLine("\nSmall dataset (1000 items):");
        var smallRange = 1000;

        sw.Restart();
        var seqSmall = Enumerable.Range(2, smallRange)
            .Where(IsPrime)
            .Count();
        sw.Stop();
        Console.WriteLine($"Sequential:  {sw.Elapsed.TotalMicroseconds:F0} us");

        sw.Restart();
        var parSmall = Enumerable.Range(2, smallRange)
            .AsParallel()
            .Where(IsPrime)
            .Count();
        sw.Stop();
        Console.WriteLine($"Parallel:    {sw.Elapsed.TotalMicroseconds:F0} us (overhead > benefit)");

        Console.WriteLine("\nDegreeOfParallelism hint:");
        sw.Restart();
        var parTuned = Enumerable.Range(2, range)
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Where(IsPrime)
            .Count();
        sw.Stop();
        Console.WriteLine($"With tuned degree: {sw.ElapsedMilliseconds} ms");
    }

    private static bool IsPrime(int number)
    {
        if (number < 2) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;
        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }
        return true;
    }
}
