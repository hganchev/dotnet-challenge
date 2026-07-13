namespace ResilienceAndRateLimiting.FlakyDependency;

/// <summary>Fails the first two calls then succeeds — simulates a transient fault that a retry
/// policy should be able to recover from.</summary>
public class FlakyService
{
    private int _attempt;

    public Task<string> CallAsync()
    {
        _attempt++;
        if (_attempt <= 2)
        {
            Console.WriteLine($"    [FlakyService] attempt {_attempt} failed");
            throw new HttpRequestException("Simulated transient failure");
        }

        Console.WriteLine($"    [FlakyService] attempt {_attempt} succeeded");
        return Task.FromResult("OK");
    }
}
