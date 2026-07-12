namespace QueryOptimization.Entities;

/// <summary>Aggregate root: a Customer places many Orders.</summary>
public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
