using BenchmarkDotNet.Attributes;
using Shared.Domain;

namespace Benchmarks;

/// <summary>
/// Compares computing an order's subtotal via LINQ's <c>Sum</c> against a hand-written
/// <c>for</c> loop, across increasing item counts. LINQ's delegate-invocation overhead is
/// usually only measurable at small sizes and negligible allocation-wise once JIT-inlined,
/// but the comparison is a good example of "measure, don't guess" for readability-vs-speed
/// tradeoffs.
/// </summary>
[MemoryDiagnoser]
public class OrderTotalBenchmarks
{
    [Params(10, 100, 1_000)]
    public int ItemCount { get; set; }

    private Order _order = null!;

    [GlobalSetup]
    public void Setup()
    {
        _order = new Order
        {
            Customer = "Benchmark Customer",
            Items = Enumerable.Range(1, ItemCount)
                .Select(i => new OrderItem { ProductName = $"Item{i}", Quantity = i % 5 + 1, UnitPrice = 9.99m })
                .ToList()
        };
    }

    [Benchmark(Baseline = true)]
    public decimal LinqSum() => _order.Items.Sum(item => item.LineTotal);

    [Benchmark]
    public decimal ForLoopSum()
    {
        decimal total = 0;
        var items = _order.Items;
        for (var i = 0; i < items.Count; i++)
        {
            total += items[i].LineTotal;
        }

        return total;
    }
}
