using CommonPatterns.CQRS.Abstractions;
using CommonPatterns.CQRS.Infrastructure;

namespace CommonPatterns.CQRS.Queries;

public sealed class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderResult?>
{
    private readonly IOrderRepository _repository;

    public GetOrderByIdQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<OrderResult?> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken = default)
    {
        var order = _repository.GetById(query.OrderId);
        if (order is null)
            return Task.FromResult<OrderResult?>(null);

        var result = new OrderResult(order.Id, order.CustomerName, order.TotalAmount, order.CreatedAt);
        return Task.FromResult<OrderResult?>(result);
    }
}
