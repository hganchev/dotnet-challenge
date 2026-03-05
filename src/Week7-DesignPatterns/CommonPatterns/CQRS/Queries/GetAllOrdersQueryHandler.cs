using CommonPatterns.CQRS.Abstractions;
using CommonPatterns.CQRS.Infrastructure;

namespace CommonPatterns.CQRS.Queries;

public sealed class GetAllOrdersQueryHandler : IQueryHandler<GetAllOrdersQuery, IEnumerable<OrderResult>>
{
    private readonly IOrderRepository _repository;

    public GetAllOrdersQueryHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<OrderResult>> HandleAsync(GetAllOrdersQuery query, CancellationToken cancellationToken = default)
    {
        var results = _repository
            .GetAll()
            .Select(o => new OrderResult(o.Id, o.CustomerName, o.TotalAmount, o.CreatedAt));

        return Task.FromResult(results);
    }
}
