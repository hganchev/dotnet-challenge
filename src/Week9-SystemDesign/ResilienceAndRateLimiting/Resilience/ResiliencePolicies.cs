using Polly;

namespace ResilienceAndRateLimiting.Resilience;

/// <summary>Builds the individual Polly policies used to demonstrate retry, circuit breaker, and
/// timeout resilience strategies.</summary>
public static class ResiliencePolicies
{
    public static IAsyncPolicy<string> CreateRetryPolicy()
    {
        var jitterer = new Random();
        return Policy<string>
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    // Exponential backoff (100ms, 200ms, 400ms, ...) plus random jitter so that many
                    // clients retrying at once don't all hammer the dependency in lockstep.
                    TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)),
                onRetry: (_, delay, attempt, _) =>
                    Console.WriteLine($"    [Retry] attempt {attempt} after {delay.TotalMilliseconds:F0}ms backoff"));
    }

    public static IAsyncPolicy<string> CreateCircuitBreakerPolicy() =>
        Policy<string>
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(2),
                onBreak: (_, breakDelay) => Console.WriteLine($"    [CircuitBreaker] OPEN for {breakDelay.TotalSeconds}s"),
                onReset: () => Console.WriteLine("    [CircuitBreaker] CLOSED (reset)"),
                onHalfOpen: () => Console.WriteLine("    [CircuitBreaker] HALF-OPEN (probing)"));

    public static IAsyncPolicy<string> CreateTimeoutPolicy() =>
        Policy.TimeoutAsync<string>(TimeSpan.FromMilliseconds(500));
}
