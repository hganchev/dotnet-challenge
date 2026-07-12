namespace VerticalSlices.Features.CreateOrder;

using VerticalSlices.SharedKernel;

/// <summary>
/// Feature folder — everything the "Create Order" use-case needs (request, handler, response)
/// lives in this single file/folder instead of being spread across Controllers/, Services/,
/// and Repositories/ folders. A change to this feature never has to touch another feature.
/// </summary>
public sealed record CreateOrderRequest(string CustomerName, decimal TotalAmount);

public sealed record CreateOrderResponse(Guid OrderId, string CustomerName, decimal TotalAmount, DateTime CreatedAt);

public sealed class CreateOrderHandler(IOrderStore store)
{
    public CreateOrderResponse Handle(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("Customer name is required.", nameof(request));
        if (request.TotalAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Total amount cannot be negative.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            TotalAmount = request.TotalAmount
        };

        store.Add(order);

        return new CreateOrderResponse(order.Id, order.CustomerName, order.TotalAmount, order.CreatedAt);
    }
}
