using Shared.Abstractions;
using Shared.Domain;

namespace Shared.Services;

/// <summary>
/// Coordinates validating, pricing and persisting an <see cref="Order"/>.
/// Depends only on abstractions, which makes it the natural target for unit tests that
/// mock <see cref="IOrderRepository"/> and <see cref="IDiscountService"/>.
/// </summary>
public class OrderService(IOrderRepository repository, IDiscountService discountService)
{
    public async Task<OrderResult> PlaceOrderAsync(Order order)
    {
        if (!order.HasItems)
        {
            throw new ArgumentException("An order must contain at least one item.", nameof(order));
        }

        if (!order.HasValidQuantities)
        {
            throw new ArgumentException("All order items must have a quantity of at least 1.", nameof(order));
        }

        var discountPercentage = discountService.GetDiscountPercentage(order);
        var total = order.Subtotal * (1 - discountPercentage / 100m);

        await repository.SaveAsync(order);

        return new OrderResult(order.Id, order.Subtotal, discountPercentage, total);
    }
}
