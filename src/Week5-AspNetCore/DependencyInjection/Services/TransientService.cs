namespace DependencyInjection.Services;

/// <summary>
/// Registered as <c>Transient</c>: a new instance is created every time it is requested
/// from the container — even multiple times within the same HTTP request.
/// </summary>
public interface ITransientService
{
    Guid InstanceId { get; }
}

public sealed class TransientService : ITransientService
{
    public Guid InstanceId { get; } = Guid.NewGuid();
}
