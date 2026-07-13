namespace ResilienceAndRateLimiting.FlakyDependency;

/// <summary>Always fails — used to demonstrate a circuit breaker tripping open and failing fast.</summary>
public class AlwaysFailingService
{
    public Task<string> CallAsync()
    {
        Console.WriteLine("    [AlwaysFailingService] call failed");
        throw new HttpRequestException("Simulated persistent failure");
    }
}
