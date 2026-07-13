using Polly.CircuitBreaker;
using Polly.Timeout;
using ResilienceAndRateLimiting.FlakyDependency;
using ResilienceAndRateLimiting.Resilience;

namespace ResilienceAndRateLimiting.Examples;

/// <summary>Demonstrates Polly retry (with exponential backoff + jitter), circuit breaker, and
/// timeout policies wrapped around simulated flaky dependencies.</summary>
public static class PollyResilienceDemo
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Resilience: Polly Retry / Circuit Breaker / Timeout ===");

        Console.WriteLine("\n-- Retry with exponential backoff + jitter --");
        var flaky = new FlakyService();
        var retryPolicy = ResiliencePolicies.CreateRetryPolicy();
        var result = await retryPolicy.ExecuteAsync(() => flaky.CallAsync());
        Console.WriteLine($"    Result: {result}");

        Console.WriteLine("\n-- Circuit breaker over a persistently failing dependency --");
        var alwaysFailing = new AlwaysFailingService();
        var breaker = ResiliencePolicies.CreateCircuitBreakerPolicy();
        for (var i = 1; i <= 4; i++)
        {
            try
            {
                await breaker.ExecuteAsync(() => alwaysFailing.CallAsync());
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine($"    [Call {i}] short-circuited — failing fast without calling the dependency");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"    [Call {i}] dependency call failed normally");
            }
        }

        Console.WriteLine("\n-- Timeout policy --");
        var timeoutPolicy = ResiliencePolicies.CreateTimeoutPolicy();
        try
        {
            await timeoutPolicy.ExecuteAsync(async ct =>
            {
                await Task.Delay(1000, ct); // slower than the 500ms timeout budget
                return "too slow";
            }, CancellationToken.None);
        }
        catch (TimeoutRejectedException)
        {
            Console.WriteLine("    [Timeout] dependency call exceeded its time budget and was aborted");
        }
    }
}
