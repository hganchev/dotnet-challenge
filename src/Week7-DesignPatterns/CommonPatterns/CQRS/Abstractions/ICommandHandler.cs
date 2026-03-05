namespace CommonPatterns.CQRS.Abstractions;

/// <summary>Handles a single command of type <typeparamref name="TCommand"/>.</summary>
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
