namespace Shared.Domain;

/// <summary>A single line item within an <see cref="Order"/>.</summary>
public class OrderItem
{
    public required string ProductName { get; init; }

    public decimal UnitPrice { get; init; }

    public int Quantity { get; init; }

    public decimal LineTotal => UnitPrice * Quantity;
}
