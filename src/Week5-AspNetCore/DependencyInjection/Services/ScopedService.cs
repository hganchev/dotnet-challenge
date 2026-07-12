namespace DependencyInjection.Services;

/// <summary>
/// Registered as <c>Scoped</c>: one instance is shared for the lifetime of a single HTTP
/// request, but a new instance is created for every new request.
/// </summary>
public interface IScopedService
{
    Guid InstanceId { get; }
}

public sealed class ScopedService : IScopedService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}
