using Shared.Domain;

namespace Shared.Abstractions;

/// <summary>Persists orders. Implemented by infrastructure; mocked in unit tests.</summary>
public interface IOrderRepository
{
    Task SaveAsync(Order order);
}
