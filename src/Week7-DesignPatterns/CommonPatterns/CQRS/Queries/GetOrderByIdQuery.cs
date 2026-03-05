using CommonPatterns.CQRS.Abstractions;

namespace CommonPatterns.CQRS.Queries;

/// <summary>Query that returns a single order by its identifier, or null when not found.</summary>
public sealed class GetOrderByIdQuery : IQuery<OrderResult?>
{
    public required Guid OrderId { get; init; }
}
