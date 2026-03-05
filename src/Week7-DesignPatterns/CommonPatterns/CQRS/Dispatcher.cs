using CommonPatterns.CQRS.Abstractions;

namespace CommonPatterns.CQRS;

/// <summary>
/// Lightweight dispatcher that routes commands and queries to registered handlers
/// using .NET generics only – no third-party mediator library required.
/// </summary>
public sealed class Dispatcher : IDispatcher
{
    private readonly Dictionary<Type, object> _commandHandlers = [];
    private readonly Dictionary<Type, object> _queryHandlers = [];

    /// <summary>Registers a command handler. Returns <c>this</c> to allow fluent chaining.</summary>
    public Dispatcher Register<TCommand>(ICommandHandler<TCommand> handler)
        where TCommand : ICommand
    {
        _commandHandlers[typeof(TCommand)] = handler;
        return this;
    }

    /// <summary>Registers a query handler. Returns <c>this</c> to allow fluent chaining.</summary>
    public Dispatcher Register<TQuery, TResult>(IQueryHandler<TQuery, TResult> handler)
        where TQuery : IQuery<TResult>
    {
        _queryHandlers[typeof(TQuery)] = handler;
        return this;
    }

    /// <inheritdoc/>
    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (!_commandHandlers.TryGetValue(typeof(TCommand), out var handler))
            throw new InvalidOperationException($"No handler registered for command '{typeof(TCommand).Name}'.");

        return ((ICommandHandler<TCommand>)handler).HandleAsync(command, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TResult> QueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        if (!_queryHandlers.TryGetValue(typeof(TQuery), out var handler))
            throw new InvalidOperationException($"No handler registered for query '{typeof(TQuery).Name}'.");

        return ((IQueryHandler<TQuery, TResult>)handler).HandleAsync(query, cancellationToken);
    }
}
