using Shared.Domain;

namespace Shared.Abstractions;

/// <summary>Computes the discount percentage (0-100) that applies to an order.</summary>
public interface IDiscountService
{
    decimal GetDiscountPercentage(Order order);
}
