namespace CommonPatterns.CQRS.Abstractions;

/// <summary>Handles a single query of type <typeparamref name="TQuery"/> and returns <typeparamref name="TResult"/>.</summary>
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
