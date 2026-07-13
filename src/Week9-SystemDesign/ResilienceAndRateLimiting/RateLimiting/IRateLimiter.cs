namespace ResilienceAndRateLimiting.RateLimiting;

/// <summary>A pluggable rate-limiting algorithm so different strategies can be swapped and compared.</summary>
public interface IRateLimiter
{
    bool TryAcquire();
}
