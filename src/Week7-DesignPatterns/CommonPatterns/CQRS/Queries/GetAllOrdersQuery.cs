using CommonPatterns.CQRS.Abstractions;

namespace CommonPatterns.CQRS.Queries;

/// <summary>Query that returns all orders.</summary>
public sealed class GetAllOrdersQuery : IQuery<IEnumerable<OrderResult>> { }
