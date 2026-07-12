namespace CleanArchitecture.Domain;

/// <summary>
/// Domain layer — the innermost layer. Zero external dependencies: no EF Core, no HTTP,
/// no DI container, no framework types. Only plain C# and business rules.
/// </summary>
public sealed class Order
{
    public Guid Id { get; }
    public string CustomerName { get; }
    private readonly List<OrderLine> _lines = [];
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    public Order(Guid id, string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));

        Id = id;
        CustomerName = customerName;
    }

    /// <summary>Business rule lives in the domain, not in a use-case or controller: an order
    /// cannot contain a non-positive quantity or a negative price.</summary>
    public void AddLine(string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");

        _lines.Add(new OrderLine(productName, quantity, unitPrice));
    }

    public decimal CalculateTotal() => _lines.Sum(l => l.Quantity * l.UnitPrice);
}

public sealed record OrderLine(string ProductName, int Quantity, decimal UnitPrice);
