using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ObservabilityAndTracing.HealthChecks;

namespace ObservabilityAndTracing.Examples;

/// <summary>Demonstrates registering both a liveness and a readiness health check via
/// <see cref="IHealthCheck"/>, and querying each independently by tag.</summary>
public static class HealthChecksDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Health Checks: Liveness vs Readiness ===");

        var services = new ServiceCollection();
        services.AddLogging(); // DefaultHealthCheckService requires ILogger<T> to be resolvable
        services.AddHealthChecks()
            .AddCheck<LivenessHealthCheck>("liveness", tags: ["live"])
            // Simulate an unreachable downstream dependency so readiness reports Unhealthy
            // even though the process itself (liveness) is fine.
            .AddCheck("readiness", new DatabaseHealthCheck(isDependencyReachable: false), tags: ["ready"]);

        await using var provider = services.BuildServiceProvider();
        var healthCheckService = provider.GetRequiredService<HealthCheckService>();

        var livenessReport = await healthCheckService.CheckHealthAsync(registration => registration.Tags.Contains("live"));
        Console.WriteLine($"Liveness:  {livenessReport.Status}");

        var readinessReport = await healthCheckService.CheckHealthAsync(registration => registration.Tags.Contains("ready"));
        var readinessDetails = string.Join(", ", readinessReport.Entries.Select(entry => $"{entry.Key}: {entry.Value.Description}"));
        Console.WriteLine($"Readiness: {readinessReport.Status} — {readinessDetails}");
    }
}
