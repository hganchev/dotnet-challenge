namespace CommonPatterns.CQRS.Domain;

public sealed class Order
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
}
