namespace CommonPatterns.CQRS.Queries;

/// <summary>Read model (DTO) returned by order queries.</summary>
public sealed record OrderResult(
    Guid Id,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt);
