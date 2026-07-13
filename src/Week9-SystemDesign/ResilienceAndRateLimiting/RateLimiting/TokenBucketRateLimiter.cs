namespace ResilienceAndRateLimiting.RateLimiting;

/// <summary>Classic token bucket: tokens refill continuously up to a capacity, and each request
/// consumes one. Allows short bursts up to the bucket's capacity, then throttles to the refill rate.</summary>
public class TokenBucketRateLimiter : IRateLimiter
{
    private readonly object _lock = new();
    private readonly double _capacity;
    private readonly double _refillRatePerSecond;
    private double _tokens;
    private DateTime _lastRefill;

    public TokenBucketRateLimiter(double capacity, double refillRatePerSecond)
    {
        _capacity = capacity;
        _refillRatePerSecond = refillRatePerSecond;
        _tokens = capacity;
        _lastRefill = DateTime.UtcNow;
    }

    public bool TryAcquire()
    {
        lock (_lock)
        {
            Refill();
            if (_tokens < 1)
            {
                return false;
            }

            _tokens -= 1;
            return true;
        }
    }

    private void Refill()
    {
        var now = DateTime.UtcNow;
        var elapsedSeconds = (now - _lastRefill).TotalSeconds;
        _tokens = Math.Min(_capacity, _tokens + (elapsedSeconds * _refillRatePerSecond));
        _lastRefill = now;
    }
}
