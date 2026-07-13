using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ObservabilityAndTracing.HealthChecks;

/// <summary>Liveness check: is the process itself still running? Deliberately has zero external
/// dependencies — a downstream outage should never make a healthy process report as dead.</summary>
public class LivenessHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy("Process is running"));
}
