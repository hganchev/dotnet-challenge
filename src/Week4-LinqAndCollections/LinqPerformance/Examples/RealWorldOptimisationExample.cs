using System.Runtime.InteropServices;

namespace LinqPerformance.Examples;

/// <summary>
/// Real-world example combining multiple optimization techniques:
/// - Streaming (lazy evaluation)
/// - Any() for early termination
/// - Span<T> for zero-copy iteration
/// - Minimal materialisation
/// </summary>
public static class RealWorldOptimisationExample
{
    public static void Run()
    {
        Console.WriteLine("\n--- Real-World Optimisation Example ---");

        // Scenario: Find first 10 products over $100 where quantity > 5
        var products = GenerateProducts(100_000);

        // Naive approach (multiple materialisation, inefficient)
        Console.WriteLine("Naive approach:");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var naive = products
            .Where(p => p.Price > 100)
            .ToList()  // Materialize all expensive items
            .Where(p => p.Quantity > 5)
            .ToList()  // Materialize again
            .Take(10)
            .ToList();  // Final list
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds} ms, Count: {naive.Count}");

        // Optimised approach (streaming + early termination)
        Console.WriteLine("Optimised approach:");
        sw.Restart();
        var optimised = products
            .Where(p => p.Price > 100)
            .Where(p => p.Quantity > 5)
            .Take(10)
            .ToList();  // Only materialise what we need
        sw.Stop();
        Console.WriteLine($"  Time: {sw.Elapsed.TotalMicroseconds:F0} us, Count: {optimised.Count}");

        // With Span iteration (for sequential access patterns)
        Console.WriteLine("Span iteration (if working with list):");
        var productList = products.ToList();
        sw.Restart();
        var optimisedSpan = new List<Product>(10);
        var span = CollectionsMarshal.AsSpan(productList);
        foreach (var product in span)
        {
            if (product.Price > 100 && product.Quantity > 5)
            {
                optimisedSpan.Add(product);
                if (optimisedSpan.Count == 10) break;
            }
        }
        sw.Stop();
        Console.WriteLine($"  Time: {sw.ElapsedMilliseconds} ms, Count: {optimisedSpan.Count}");

        Console.WriteLine("\nKey takeaways:");
        Console.WriteLine("  1. Stream filters, materialise only at the end");
        Console.WriteLine("  2. Use Any() instead of Count() > 0");
        Console.WriteLine("  3. Use Take() to limit results early");
        Console.WriteLine("  4. Use Span<T> for sequential access");
        Console.WriteLine("  5. Chain operations without intermediate ToList()");
    }

    private record Product(string Name, decimal Price, int Quantity);

    private static IEnumerable<Product> GenerateProducts(int count)
    {
        var random = new Random(42);
        for (int i = 0; i < count; i++)
        {
            yield return new Product(
                $"Product{i}",
                random.Next(1, 200),
                random.Next(0, 100)
            );
        }
    }
}
