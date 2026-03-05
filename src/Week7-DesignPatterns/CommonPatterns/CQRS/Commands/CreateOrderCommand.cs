using CommonPatterns.CQRS.Abstractions;

namespace CommonPatterns.CQRS.Commands;

/// <summary>Command to create a new order.
/// The caller supplies the <see cref="OrderId"/> so it is known before the handler runs –
/// a common pattern when you need the identity on the write side without a round-trip query.</summary>
public sealed class CreateOrderCommand : ICommand
{
    public Guid OrderId { get; init; } = Guid.NewGuid();
    public required string CustomerName { get; init; }
    public required decimal TotalAmount { get; init; }
}
