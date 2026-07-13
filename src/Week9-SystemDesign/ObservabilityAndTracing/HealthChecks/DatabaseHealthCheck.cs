using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ObservabilityAndTracing.HealthChecks;

/// <summary>Readiness check: can this instance actually serve traffic right now? Depends on a
/// downstream dependency (e.g. a database), unlike a liveness check.</summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly bool _isDependencyReachable;

    public DatabaseHealthCheck(bool isDependencyReachable) => _isDependencyReachable = isDependencyReachable;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(_isDependencyReachable
            ? HealthCheckResult.Healthy("Database reachable")
            : HealthCheckResult.Unhealthy("Database connection failed"));
}
