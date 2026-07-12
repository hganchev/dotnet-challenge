namespace LinqInternals.Examples;

/// <summary>
/// Demonstrates relational/set-style LINQ operators — <c>GroupBy</c>,
/// <c>Join</c>, <c>Except</c>, <c>Intersect</c>, <c>Union</c> — as the
/// declarative alternative to hand-written nested loops.
/// </summary>
public static class SetAndJoinOperatorsExample
{
    private record Customer(int Id, string Name);
    private record Order(int Id, int CustomerId, decimal Total);

    public static void Run()
    {
        Console.WriteLine("\n--- Set & Relational Operators (GroupBy, Join, Except, Intersect, Union) ---");

        var customers = new[]
        {
            new Customer(1, "Alice"),
            new Customer(2, "Bob"),
            new Customer(3, "Charlie"),
        };

        var orders = new[]
        {
            new Order(100, 1, 25.50m),
            new Order(101, 1, 12.00m),
            new Order(102, 2, 40.00m),
        };

        // Join: relational-style match between two sequences on a shared key.
        Console.WriteLine("Join (customers with their orders):");
        var joined = customers.Join(
            orders,
            customer => customer.Id,
            order => order.CustomerId,
            (customer, order) => $"{customer.Name} -> Order #{order.Id} ({order.Total:C})");
        foreach (var line in joined)
        {
            Console.WriteLine($"  {line}");
        }

        // GroupBy: aggregate orders per customer without a manual dictionary loop.
        Console.WriteLine("\nGroupBy (order count and total per customer):");
        var grouped = orders
            .GroupBy(o => o.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count(), Total = g.Sum(o => o.Total) });
        foreach (var g in grouped)
        {
            Console.WriteLine($"  Customer {g.CustomerId}: {g.Count} order(s), total {g.Total:C}");
        }

        // Except / Intersect / Union: set algebra over sequences.
        var setA = new[] { 1, 2, 3, 4 };
        var setB = new[] { 3, 4, 5, 6 };

        Console.WriteLine($"\nExcept (A not in B):    {string.Join(", ", setA.Except(setB))}");
        Console.WriteLine($"Intersect (A and B):    {string.Join(", ", setA.Intersect(setB))}");
        Console.WriteLine($"Union (A or B, no dups): {string.Join(", ", setA.Union(setB))}");

        // Customers who have NOT placed any order — Except by projected key.
        var customersWithoutOrders = customers
            .Select(c => c.Id)
            .Except(orders.Select(o => o.CustomerId));
        var namesWithoutOrders = customers
            .Where(c => customersWithoutOrders.Contains(c.Id))
            .Select(c => c.Name);
        Console.WriteLine($"\nCustomers with no orders: {string.Join(", ", namesWithoutOrders)}");
    }
}
