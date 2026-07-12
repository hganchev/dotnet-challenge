namespace QueryOptimization.Entities;

/// <summary>An Order belongs to one Customer and contains many OrderItems.</summary>
public class Order
{
    public int Id { get; set; }

    public DateTime PlacedAt { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
