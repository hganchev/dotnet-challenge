namespace Shared.Domain;

/// <summary>Outcome of successfully placing an order.</summary>
public record OrderResult(Guid OrderId, decimal Subtotal, decimal DiscountPercentage, decimal Total);
