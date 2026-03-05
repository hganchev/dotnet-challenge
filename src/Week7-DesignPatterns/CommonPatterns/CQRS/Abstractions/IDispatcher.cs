namespace CommonPatterns.CQRS.Abstractions;

/// <summary>Routes commands and queries to their registered handlers.</summary>
public interface IDispatcher
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>;
}
