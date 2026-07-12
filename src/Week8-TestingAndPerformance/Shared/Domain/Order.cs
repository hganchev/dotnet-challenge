namespace Shared.Domain;

/// <summary>A customer order composed of one or more <see cref="OrderItem"/> lines.</summary>
public class Order
{
    private const int MinimumQuantity = 1;

    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Customer { get; init; }

    public List<OrderItem> Items { get; init; } = [];

    /// <summary>Sum of all line totals, before any discount is applied.</summary>
    public decimal Subtotal => Items.Sum(item => item.LineTotal);

    public bool HasItems => Items.Count > 0;

    public bool HasValidQuantities => Items.All(item => item.Quantity >= MinimumQuantity);
}
