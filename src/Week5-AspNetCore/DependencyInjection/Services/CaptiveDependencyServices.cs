namespace DependencyInjection.Services;

/// <summary>
/// Demonstrates a <b>captive dependency</b> bug: a Singleton that captures a Scoped
/// service in its constructor. The Scoped instance gets "trapped" at first-resolution
/// time and is then reused for the app's entire lifetime instead of once per request —
/// producing stale data. This class is registered as Singleton on purpose to show the
/// symptom; see <see cref="FixedCaptiveDependencyService"/> for the corrected pattern.
/// </summary>
public interface IBuggyCaptiveDependencyService
{
    Guid CapturedScopedInstanceId { get; }
}

public sealed class BuggyCaptiveDependencyService : IBuggyCaptiveDependencyService
{
    // Captured ONCE, at Singleton construction time — never updated per request.
    public Guid CapturedScopedInstanceId { get; }

    public BuggyCaptiveDependencyService(IScopedService scopedService)
    {
        CapturedScopedInstanceId = scopedService.InstanceId;
    }
}

/// <summary>
/// Fix for the captive dependency bug: instead of injecting <see cref="IScopedService"/>
/// directly into a Singleton constructor, inject <see cref="IServiceScopeFactory"/> and
/// create a fresh scope each time the scoped dependency is actually needed.
/// </summary>
public interface IFixedCaptiveDependencyService
{
    Guid ResolveCurrentScopedInstanceId();
}

public sealed class FixedCaptiveDependencyService : IFixedCaptiveDependencyService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public FixedCaptiveDependencyService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Guid ResolveCurrentScopedInstanceId()
    {
        using var scope = _scopeFactory.CreateScope();
        var scopedService = scope.ServiceProvider.GetRequiredService<IScopedService>();
        return scopedService.InstanceId;
    }
}
