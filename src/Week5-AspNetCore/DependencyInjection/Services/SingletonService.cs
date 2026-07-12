namespace DependencyInjection.Services;

/// <summary>
/// Registered as <c>Singleton</c>: exactly one instance is created for the entire
/// application lifetime and shared across every request. Must be thread-safe.
/// </summary>
public interface ISingletonService
{
    Guid InstanceId { get; }
}

public sealed class SingletonService : ISingletonService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}
