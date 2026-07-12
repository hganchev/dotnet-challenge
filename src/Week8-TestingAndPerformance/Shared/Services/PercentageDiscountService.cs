using Shared.Abstractions;
using Shared.Domain;

namespace Shared.Services;

/// <summary>
/// Grants a bulk discount once the order subtotal crosses a threshold.
/// Kept simple and pure so it is trivial to swap for a mock in unit tests.
/// </summary>
public class PercentageDiscountService : IDiscountService
{
    private const decimal BulkOrderThreshold = 100m;
    private const decimal BulkDiscountPercentage = 10m;

    public decimal GetDiscountPercentage(Order order) =>
        order.Subtotal >= BulkOrderThreshold ? BulkDiscountPercentage : 0m;
}
