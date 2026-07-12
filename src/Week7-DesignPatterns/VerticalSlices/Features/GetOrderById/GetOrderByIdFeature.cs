namespace VerticalSlices.Features.GetOrderById;

using VerticalSlices.SharedKernel;

/// <summary>
/// Feature folder — self-contained "Get Order By Id" query slice. Note this feature defines
/// its own response shape (<see cref="GetOrderByIdResponse"/>) even though it looks similar to
/// CreateOrder's response — duplication here is preferred over a shared DTO that would couple
/// the two unrelated features together.
/// </summary>
public sealed record GetOrderByIdRequest(Guid OrderId);

public sealed record GetOrderByIdResponse(Guid OrderId, string CustomerName, decimal TotalAmount, DateTime CreatedAt);

public sealed class GetOrderByIdHandler(IOrderStore store)
{
    public GetOrderByIdResponse? Handle(GetOrderByIdRequest request)
    {
        var order = store.Find(request.OrderId);
        return order is null
            ? null
            : new GetOrderByIdResponse(order.Id, order.CustomerName, order.TotalAmount, order.CreatedAt);
    }
}
