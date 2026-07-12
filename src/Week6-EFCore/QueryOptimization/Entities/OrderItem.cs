namespace QueryOptimization.Entities;

/// <summary>A single line item within an Order.</summary>
public class OrderItem
{
    public int Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;
}
