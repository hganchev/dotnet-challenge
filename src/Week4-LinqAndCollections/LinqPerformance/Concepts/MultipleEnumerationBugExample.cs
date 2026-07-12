namespace LinqPerformance.Concepts;

/// <summary>
/// Demonstrates the multiple enumeration bug — iterating the same IEnumerable<T>
/// multiple times evaluates the query multiple times, wasting CPU and memory.
/// </summary>
public static class MultipleEnumerationBugExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Multiple Enumeration Bug ---");

        var numbers = Enumerable.Range(1, 100);
        var evenNumbers = numbers
            .Where(n =>
            {
                Console.WriteLine($"  Evaluating {n}");
                return n % 2 == 0;
            });

        Console.WriteLine("First iteration:");
        var firstCount = evenNumbers.Count();
        Console.WriteLine($"Count: {firstCount}\n");

        Console.WriteLine("Second iteration (query evaluates AGAIN!):");
        var secondCount = evenNumbers.Count();
        Console.WriteLine($"Count: {secondCount}\n");

        Console.WriteLine("The WHERE runs twice! This is the bug.\n");

        Console.WriteLine("Fix: Materialize with ToList()");
        var materialized = evenNumbers.ToList();
        Console.WriteLine($"First: {materialized.Count}");
        Console.WriteLine($"Second: {materialized.Count}");
        Console.WriteLine("WHERE only ran once!");
    }
}
