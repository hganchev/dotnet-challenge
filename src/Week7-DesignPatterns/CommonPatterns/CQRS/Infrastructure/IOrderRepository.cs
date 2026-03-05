using CommonPatterns.CQRS.Domain;

namespace CommonPatterns.CQRS.Infrastructure;

public interface IOrderRepository
{
    void Add(Order order);
    Order? GetById(Guid id);
    IEnumerable<Order> GetAll();
}
