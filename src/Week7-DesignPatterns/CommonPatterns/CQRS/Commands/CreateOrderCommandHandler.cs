using CommonPatterns.CQRS.Abstractions;
using CommonPatterns.CQRS.Domain;
using CommonPatterns.CQRS.Infrastructure;

namespace CommonPatterns.CQRS.Commands;

public sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    private readonly IOrderRepository _repository;

    public CreateOrderCommandHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Id = command.OrderId,
            CustomerName = command.CustomerName,
            TotalAmount = command.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        _repository.Add(order);
        Console.WriteLine($"  [CreateOrderCommandHandler] Order created » Id: {order.Id}, Customer: {order.CustomerName}, Amount: {order.TotalAmount:C}");
        return Task.CompletedTask;
    }
}
